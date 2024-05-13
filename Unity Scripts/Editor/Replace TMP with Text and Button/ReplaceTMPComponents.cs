using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MCG.UnityCheatSheet.Editor
{
    public class ReplaceTMPComponents : EditorWindow
    {
        [MenuItem("Tools/Replace TMP Components")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ReplaceTMPComponents));
        }

        private void OnGUI()
        {
            GUILayout.Label("Replace TMP Components", EditorStyles.boldLabel);

            if (GUILayout.Button("Replace Components"))
            {
                ReplaceComponents();
            }
        }

        private void ReplaceComponents()
        {
            TMP_Text[] tmpTextComponents = FindObjectsOfType<TMP_Text>();
            foreach (TMP_Text tmpText in tmpTextComponents)
            {
                GameObject gameObject = tmpText.gameObject;

                // Create a new GameObject for the Text component
                GameObject textObject = new GameObject("Text");
                textObject.transform.SetParent(gameObject.transform.parent, false);
                textObject.transform.localPosition = gameObject.transform.localPosition;
                textObject.transform.localRotation = gameObject.transform.localRotation;
                textObject.transform.localScale = gameObject.transform.localScale;

                // Add Unity UI Text component to the new GameObject
                Text textComponent = textObject.AddComponent<Text>();
                textComponent.text = tmpText.text;
                textComponent.color = tmpText.color;
                textComponent.alignment = TextAnchor.MiddleCenter; // Center alignment
                textComponent.fontSize = Mathf.RoundToInt(tmpText.fontSize);
                textComponent.fontStyle = (FontStyle)tmpText.fontStyle;
                textComponent.raycastTarget = tmpText.raycastTarget;

                // Copy RectTransform properties
                RectTransform tmpRectTransform = tmpText.GetComponent<RectTransform>();
                RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
                textRectTransform.anchorMin = tmpRectTransform.anchorMin;
                textRectTransform.anchorMax = tmpRectTransform.anchorMax;
                textRectTransform.pivot = tmpRectTransform.pivot;
                textRectTransform.anchoredPosition = tmpRectTransform.anchoredPosition;
                textRectTransform.sizeDelta = tmpRectTransform.sizeDelta;

                // Remove TMP_Text component
                DestroyImmediate(tmpText);

                // Rename the old GameObject to "Text"
                gameObject.name = "Text";
            }

            TextMeshProUGUI[] tmpButtonComponents = FindObjectsOfType<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmpButton in tmpButtonComponents)
            {
                GameObject gameObject = tmpButton.gameObject;

                // Create a new GameObject for the Button component
                GameObject buttonObject = new GameObject("Button");
                buttonObject.transform.SetParent(gameObject.transform.parent, false);
                buttonObject.transform.localPosition = gameObject.transform.localPosition;
                buttonObject.transform.localRotation = gameObject.transform.localRotation;
                buttonObject.transform.localScale = gameObject.transform.localScale;

                // Add Unity UI Button component to the new GameObject
                Button buttonComponent = buttonObject.AddComponent<Button>();

                // Copy RectTransform properties
                RectTransform tmpRectTransform = tmpButton.GetComponent<RectTransform>();
                RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
                buttonRectTransform.anchorMin = tmpRectTransform.anchorMin;
                buttonRectTransform.anchorMax = tmpRectTransform.anchorMax;
                buttonRectTransform.pivot = tmpRectTransform.pivot;
                buttonRectTransform.anchoredPosition = tmpRectTransform.anchoredPosition;
                buttonRectTransform.sizeDelta = tmpRectTransform.sizeDelta;

                // Remove TextMeshProUGUI component
                DestroyImmediate(tmpButton);

                // Rename the old GameObject to "Button"
                gameObject.name = "Button";
            }

            Debug.Log("TMP components replaced with Unity UI components.");
        }
    }
}
