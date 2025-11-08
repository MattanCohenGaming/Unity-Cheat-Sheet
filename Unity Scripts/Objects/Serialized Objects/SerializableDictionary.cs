using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MCG.UnityCheatSheet
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        // Backing lists shown in Inspector
        [SerializeField, FormerlySerializedAs("keys")]
        private List<TKey> keyList = new List<TKey>();

        [SerializeField, FormerlySerializedAs("values")]
        private List<TValue> valueList = new List<TValue>();

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // In edit mode, do NOT overwrite the lists from the runtime dictionary.
            // This allows the + button to add a new row that persists through ApplyModifiedProperties.
            if (!Application.isPlaying)
                return;
#endif
            keyList.Clear();
            valueList.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keyList.Add(pair.Key);
                valueList.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            // Keep lists in sync to avoid errors from inspector edits
#if UNITY_EDITOR
            if (keyList.Count > valueList.Count)
            {
                int toAdd = keyList.Count - valueList.Count;
                for (int i = 0; i < toAdd; i++)
                    valueList.Add(default(TValue));
            }
            else if (valueList.Count > keyList.Count)
            {
                int toAdd = valueList.Count - keyList.Count;
                for (int i = 0; i < toAdd; i++)
                    keyList.Add(default(TKey));
            }
#else
            int min = Math.Min(keyList.Count, valueList.Count);
            if (keyList.Count > min)
                keyList.RemoveRange(min, keyList.Count - min);
            if (valueList.Count > min)
                valueList.RemoveRange(min, valueList.Count - min);
#endif

            int count = Math.Min(keyList.Count, valueList.Count);
            for (int i = 0; i < count; i++)
            {
                var key = keyList[i];
                var value = valueList[i];

                // Skip null/invalid keys so Dictionary.Add never sees a null
                if (IsNullKey(key))
                    continue;

                if (this.ContainsKey(key))
                    this[key] = value;
                else
                    this.Add(key, value);
            }
        }

        private static bool IsNullKey(TKey key)
        {
            if (key == null) return true;
            if (key is UnityEngine.Object uo && uo == null) return true;
            return false;
        }
    }
}
