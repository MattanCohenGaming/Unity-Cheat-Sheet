#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(MCG.UnityCheatSheet.SerializableDictionary<,>), true)]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    private ReorderableList list;
    private SerializedProperty keysProp;
    private SerializedProperty valuesProp;
    private Type keyType;
    private Type valueType;
    private string boundPropertyPath;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Never mutate here
        return list != null ? list.GetHeight() : EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnsureList(property);

        if (list == null)
        {
            EditorGUI.LabelField(position, label.text, "Unsupported dictionary");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);
        list.DoList(position);
        HandleDragAndDrop(position, property);
        EditorGUI.EndProperty();
    }

    private void EnsureList(SerializedProperty property)
    {
        // Rebuild when first time, after domain reload, or when property binding changes
        if (list != null && boundPropertyPath == property.propertyPath)
            return;

        keysProp = property.FindPropertyRelative("keyList");
        valuesProp = property.FindPropertyRelative("valueList");
        if (keysProp == null || valuesProp == null) { list = null; return; }
        boundPropertyPath = property.propertyPath;

        // Cache generic type args
        keyType = null;
        valueType = null;
        if (fieldInfo != null && fieldInfo.FieldType.IsGenericType)
        {
            var args = fieldInfo.FieldType.GetGenericArguments();
            if (args.Length == 2) { keyType = args[0]; valueType = args[1]; }
        }

        SyncSizes(property.serializedObject.targetObject);

        list = new ReorderableList(property.serializedObject, keysProp, true, true, true, true);

        list.drawHeaderCallback = rect =>
        {
            float half = rect.width / 2f;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, half - 6f, rect.height), $"{property.displayName} (Key)");
            EditorGUI.LabelField(new Rect(rect.x + half, rect.y, half - 6f, rect.height), "Value");
        };

        list.drawElementCallback = (rect, index, active, focused) =>
        {
            if (index < 0 || index >= keysProp.arraySize || index >= valuesProp.arraySize) return;

            const float pad = 4f;
            float half = (rect.width - pad) / 2f;
            var keyRect = new Rect(rect.x, rect.y + 2f, half, EditorGUIUtility.singleLineHeight);
            var valRect = new Rect(rect.x + half + pad, rect.y + 2f, half, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(keyRect, keysProp.GetArrayElementAtIndex(index), GUIContent.none, true);
            EditorGUI.PropertyField(valRect, valuesProp.GetArrayElementAtIndex(index), GUIContent.none, true);
        };

        list.onAddCallback = l =>
        {
            var target = property.serializedObject.targetObject;
            Undo.RecordObject(target, "Add Dictionary Entry");
            keysProp.arraySize++;
            valuesProp.arraySize++;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        };

        list.onRemoveCallback = l =>
        {
            int i = l.index;
            if (i < 0 || i >= keysProp.arraySize) return;
            var target = property.serializedObject.targetObject;
            Undo.RecordObject(target, "Remove Dictionary Entry");

            SafeDelete(keysProp, i);
            SafeDelete(valuesProp, i);

            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);

            // Clamp selection safely when list may be empty
            l.index = Mathf.Clamp(i - 1, 0, Mathf.Max(0, keysProp.arraySize - 1));
        };

        list.onReorderCallbackWithDetails = (l, oldIndex, newIndex) =>
        {
            var target = property.serializedObject.targetObject;
            Undo.RecordObject(target, "Reorder Dictionary Entry");
            // Keep values in the same order as keys
            valuesProp.MoveArrayElement(oldIndex, newIndex);
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        };
    }

    private void SyncSizes(UnityEngine.Object target)
    {
        if (keysProp.arraySize == valuesProp.arraySize) return;
        Undo.RecordObject(target, "Sync Dictionary Lists");
        if (keysProp.arraySize > valuesProp.arraySize)
            valuesProp.arraySize = keysProp.arraySize;
        else
            keysProp.arraySize = valuesProp.arraySize;
        keysProp.serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private static void SafeDelete(SerializedProperty array, int index)
    {
        if (index < 0 || index >= array.arraySize) return;

        // For ObjectReference arrays, first null, then delete
        var el = array.GetArrayElementAtIndex(index);
        if (el.propertyType == SerializedPropertyType.ObjectReference && el.objectReferenceValue != null)
        {
            el.objectReferenceValue = null;
            array.serializedObject.ApplyModifiedProperties();
        }

        array.DeleteArrayElementAtIndex(index);

        // If after delete, the element still exists (because first delete only nulled it), delete again
        if (index < array.arraySize)
        {
            var check = array.GetArrayElementAtIndex(index);
            if (check.propertyType == SerializedPropertyType.ObjectReference && check.objectReferenceValue == null)
                array.DeleteArrayElementAtIndex(index);
        }
    }

    private void HandleDragAndDrop(Rect position, SerializedProperty property)
    {
        var evt = Event.current;
        if (evt == null) return;

        // Build drop zones that match the visual list
        float headerH = list.headerHeight > 0 ? list.headerHeight : EditorGUIUtility.singleLineHeight;
        float footerH = list.footerHeight > 0 ? list.footerHeight : 13f;
        float elemsH = Mathf.Max(list.elementHeight, list.elementHeight * Mathf.Max(1, keysProp.arraySize)); // at least one row space
        float totalH = headerH + elemsH + footerH;

        var listRect = new Rect(position.x, position.y, position.width, totalH);
        if (!listRect.Contains(evt.mousePosition)) return;

        var elementsRect = new Rect(position.x, position.y + headerH, position.width, elemsH);
        bool onLeftColumn = evt.mousePosition.x < (elementsRect.x + elementsRect.width * 0.5f);

        var tgtType = onLeftColumn ? keyType : valueType;
        bool acceptsString = tgtType == typeof(string);
        bool acceptsObjRef = tgtType != null && typeof(UnityEngine.Object).IsAssignableFrom(tgtType);

        var dragged = DragAndDrop.objectReferences;
        if (dragged == null || dragged.Length == 0) return;

        // See if any dragged object can be converted/assigned
        bool canAccept = acceptsString;
        if (!canAccept && acceptsObjRef)
        {
            foreach (var o in dragged)
            {
                if (ConvertForExpected(o, tgtType) != null) { canAccept = true; break; }
            }
        }
        if (!canAccept) return;

        if (evt.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            evt.Use();
            return;
        }

        if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            var target = property.serializedObject.targetObject;
            Undo.RecordObject(target, onLeftColumn ? "Add Keys From Drag" : "Add Values From Drag");

            foreach (var o in dragged)
            {
                int newIndex = keysProp.arraySize;
                // Grow both sides (pair alignment)
                keysProp.arraySize++;
                valuesProp.arraySize++;

                var element = onLeftColumn
                    ? keysProp.GetArrayElementAtIndex(newIndex)
                    : valuesProp.GetArrayElementAtIndex(newIndex);

                if (acceptsString && element.propertyType == SerializedPropertyType.String)
                {
                    element.stringValue = o != null ? o.name : string.Empty;
                }
                else if (acceptsObjRef && element.propertyType == SerializedPropertyType.ObjectReference)
                {
                    var assigned = ConvertForExpected(o, tgtType);
                    if (assigned != null) element.objectReferenceValue = assigned;
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            evt.Use();
        }
    }

    private static UnityEngine.Object ConvertForExpected(UnityEngine.Object obj, Type expectedType)
    {
        if (obj == null || expectedType == null) return null;

        // Direct assign
        if (expectedType.IsAssignableFrom(obj.GetType()))
            return obj;

        // GameObject -> Component (incl. Transform)
        if (obj is GameObject go)
        {
            if (expectedType == typeof(Transform)) return go.transform;
            if (typeof(Component).IsAssignableFrom(expectedType))
            {
                var comp = go.GetComponent(expectedType) as Component;
                return comp;
            }
        }

        // Component -> GameObject or another Component type
        if (obj is Component c)
        {
            if (expectedType == typeof(GameObject)) return c.gameObject;
            if (expectedType == typeof(Transform)) return c.transform;
            if (expectedType.IsAssignableFrom(c.GetType())) return c;
            if (typeof(Component).IsAssignableFrom(expectedType))
            {
                var comp = c.gameObject.GetComponent(expectedType) as Component;
                return comp;
            }
        }

        return null;
    }
}
#endif