using System;
using UnityEditor;
using UnityEngine;

namespace MCG.UnityCheatSheet.Editor
{
    public static class CustomDialog
    {
        public static void DisplayDialog(
            string title,
            string text,
            string yesText = "Ok",
            string noText = null
        )
        {
            if (!string.IsNullOrEmpty(noText))
                EditorUtility.DisplayDialog(title, text, yesText, noText);
            else
                EditorUtility.DisplayDialog(title, text, yesText);
        }

        public static void ConfirmationDialog(
            string title,
            string text,
            Action onYes,
            Action onNo = null,
            string yesText = "Yes",
            string noText = "No"
        )
        {
            bool confirmed = EditorUtility.DisplayDialog(title, text, yesText, noText);

            if (confirmed && onYes != null)
                onYes?.Invoke();
            else if (onNo != null)
                onNo?.Invoke();
        }

        public static void SaveFileDialog(
            Action<string> onFileSelected,
            string title = "Save File As",
            string directory = "",
            string extension = "txt",
            string defaultName = "My File"
        )
        {
            string path = EditorUtility.SaveFilePanel(title, directory, defaultName, extension);

            // Check if a path was selected (Cancel button results in an empty string)
            if (!string.IsNullOrEmpty(path))
            {
                onFileSelected?.Invoke(path);
            }
        }

        public static void SaveFileInProjectDialog(
            Action<string> onFileSelected,
            string title = "Save File As",
            string defaultName = "My File",
            string extension = "txt",
            string message = ""
        )
        {
            string path = EditorUtility.SaveFilePanelInProject(
                title: title,
                defaultName: defaultName,
                extension: extension,
                message: message
            );

            // Check if a path was selected (Cancel button results in an empty string)
            if (!string.IsNullOrEmpty(path))
            {
                onFileSelected?.Invoke(path);
            }
        }

        public static void LoadFileDialog(
            Action<string> onFileSelected,
            string title = "Open File",
            string directory = "",
            string extension = "txt,json"
        )
        {
            string path = EditorUtility.OpenFilePanel(title, directory, extension);

            // Check if a path was selected (Cancel button results in an empty string)
            if (!string.IsNullOrEmpty(path))
            {
                onFileSelected?.Invoke(path);
            }
        }

        public static void LoadFileInProjectDialog(
            Action<string> onFileSelected,
            string title = "Open File",
            string directory = "",
            string extension = "txt,json"
        )
        {
            string path = EditorUtility.OpenFilePanel(title, directory, extension);

            // Check if a path was selected (Cancel button results in an empty string)
            if (!string.IsNullOrEmpty(path))
            {
                var pathRelativeToProject = "Assets" + path.Replace(Application.dataPath, "");
                onFileSelected?.Invoke(pathRelativeToProject);
            }
        }
    }
}
