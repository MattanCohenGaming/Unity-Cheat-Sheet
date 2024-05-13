using System.Collections.Generic;
using Newledge.Core.Utils.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace MCG.UnityCheatSheet.Editor
{
    public class PackageUpdater
    {
        private static ListRequest listRequest;
        private static Queue<string> packageNames = new Queue<string>();
        private static AddRequest addRequest;

        [MenuItem("Newledge/Core/Extensions/Update All Packages")]
        private static void UpdateNewledgePackages()
        {
            Debug.Log("Starting to update all Newledge packages.");
            listRequest = Client.List(true); // True to include all packages (not just direct dependencies)
            EditorApplication.update += ProgressListRequest;
        }

        private static void ProgressListRequest()
        {
            if (listRequest.IsCompleted)
            {
                EditorApplication.update -= ProgressListRequest;
                if (listRequest.Status == StatusCode.Success)
                {
                    foreach (var package in listRequest.Result)
                    {
                        if (package.name.Contains("newledge") && !package.name.Contains("core"))
                        {
                            packageNames.Enqueue(package.packageId);
                        }
                    }
                    UpdateNextPackage(); // Start updating packages one by one
                }
                else
                {
                    Debug.LogError("Failed to list packages: " + listRequest.Error.message);
                }
            }
        }

        private static void UpdateNextPackage()
        {
            if (packageNames.Count > 0)
            {
                string packageName = packageNames.Dequeue();
                addRequest = Client.Add(packageName);
                EditorApplication.update += CheckAddRequestCompletion;
            }
            else
            {
                FinishedUpdate();
            }
        }

        private static void CheckAddRequestCompletion()
        {
            if (addRequest.IsCompleted)
            {
                EditorApplication.update -= CheckAddRequestCompletion;
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log("Updated package: " + addRequest.Result.packageId);
                }
                else
                {
                    Debug.LogError("Failed to update package: " + addRequest.Error.message);
                }
                UpdateNextPackage(); // Continue with the next package
            }
        }

        private static void FinishedUpdate()
        {
            Debug.Log(
                "Finished updating all packages. The Newledge Core package won't be updated. To update it, please do it manually."
            );
            CustomDialog.ConfirmationDialog(
                title: "Restart After Update",
                text: "The editor needs to be restarted after the update. Restart now?",
                onYes: () =>
                {
                    string projectPath = System.IO.Path.GetDirectoryName(Application.dataPath);
                    Debug.Log("Restarting Unity Editor...");
                    EditorApplication.OpenProject(projectPath);
                }
            );
        }
    }
}
