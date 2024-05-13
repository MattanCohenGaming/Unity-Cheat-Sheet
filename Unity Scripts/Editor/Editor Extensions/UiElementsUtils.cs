using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MCG.UnityCheatSheet.Editor
{
    public static class UiElementsUtils
    {
        public static void Hide(VisualElement element) => element.style.display = DisplayStyle.None;

        public static void Show(VisualElement element) => element.style.display = DisplayStyle.Flex;

        public static void ChangeVisibility(bool show, VisualElement element)
        {
            if (show)
                Show(element);
            else
                Hide(element);
        }

        public static void SetBackgroundColor(string hexColor, VisualElement visualElement)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hexColor, out color))
            {
                visualElement.style.backgroundColor = new StyleColor(color);
            }
            else
            {
                Debug.LogError("Invalid color format: " + hexColor);
            }
        }

        public static void SetBackgroundColor(Color color, VisualElement visualElement)
        {
            visualElement.style.backgroundColor = new StyleColor(color);
        }

        public static VisualTreeAsset LoadUXML(string uxmlFilePath)
        {
            if (string.IsNullOrEmpty(uxmlFilePath))
                return null;
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlFilePath);
        }

        public static StyleSheet LoadUSS(string ussFilePath)
        {
            if (string.IsNullOrEmpty(ussFilePath))
                return null;
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(ussFilePath);
        }

        public static VisualElement FromUXML(string uxmlFilePath, string ussFilePath = null)
        {
            VisualElement visualElement = new VisualElement();

            if (!string.IsNullOrEmpty(uxmlFilePath))
            {
                // load the visual tree from UXML
                var visualTree = LoadUXML(uxmlFilePath);
                visualTree.CloneTree(visualElement);
                // load and apply USS styles
                if (!string.IsNullOrEmpty(ussFilePath))
                {
                    var styleSheet = LoadUSS(ussFilePath);
                    visualElement.styleSheets.Add(styleSheet);
                }
            }

            return visualElement;
        }
    }
}
