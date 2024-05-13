using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MCG.UnityCheatSheet.Editor
{
    public static class GUILayoutUtils
    {
        #region Label
        public static void Label(
            string text = "",
            string boldPrefixLabel = "",
            bool isVertical = true
        )
        {
            if (isVertical)
                GUILayout.BeginVertical();
            else
                GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(boldPrefixLabel))
                BoldLabel(boldPrefixLabel);
            GUILayout.Label(text);

            if (isVertical)
                GUILayout.EndVertical();
            else
                GUILayout.EndHorizontal();
        }

        public static void BoldLabel(string text = "")
        {
            GUIStyle style = new GUIStyle() { fontSize = 14, fontStyle = FontStyle.Bold };
            style.normal.textColor = Color.white;
            GUILayout.Label(text, style);
        }
        #endregion

        #region Text Field
        public static string TextField(string text, params GUILayoutOption[] options) =>
            GUILayout.TextField(text, options);

        // Modified TextField with label size percentage
        public static string TextField(
            string label,
            string text,
            float labelSizePercentage = 5f,
            params GUILayoutOption[] options
        )
        {
            var labelWidthLayoutOption = GUILayout.Width(GetLabelWidth(labelSizePercentage));
            options.Append(labelWidthLayoutOption); // add label width to option

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, options);
            var result = TextField(text);
            GUILayout.EndHorizontal();

            return result;
        }
        #endregion

        #region Text Area
        public static string TextArea(string text, params GUILayoutOption[] options) =>
            GUILayout.TextArea(text, options);

        // Modified TextArea with label size percentage
        public static string TextArea(
            string label,
            string text,
            float labelSizePercentage = 5f,
            params GUILayoutOption[] options
        )
        {
            var labelWidthLayoutOption = GUILayout.Width(GetLabelWidth(labelSizePercentage));
            options.Append(labelWidthLayoutOption); // add label width to option

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(GetLabelWidth(labelSizePercentage)));
            var result = TextArea(text, options);
            GUILayout.EndHorizontal();

            return result;
        }
        #endregion

        #region Helpers
        // Helper method to calculate label width based on the percentage
        private static float GetLabelWidth(float percentage)
        {
            var totalWidth = EditorGUIUtility.currentViewWidth; // Get the current view width
            return totalWidth * percentage / 100; // Calculate label width as a percentage of total width
        }
        #endregion
    }
}
