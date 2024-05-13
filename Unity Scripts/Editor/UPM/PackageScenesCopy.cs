using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newledge.Core.Utils.Editor;
using UnityEditor;
using UnityEditor.PackageManager;

namespace MCG.UnityCheatSheet.Editor
{
    [InitializeOnLoad]
    public class PackageScenesCopy
    {
        static PackageScenesCopy()
        {
            SubscribeToNewPackages();
        }

        [MenuItem("Newledge/Core/Extensions/Try Copying Scenes From Packages")]
        public static void CheckPackages()
        {
            var packages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
            OnPackagesChanged(packages.ToList());
        }

        private static void SubscribeToNewPackages()
        {
            Events.registeredPackages += (args) => OnPackagesChanged(args.added.ToList());
        }

        private static void OnPackagesChanged(List<UnityEditor.PackageManager.PackageInfo> args)
        {
            foreach (var package in args)
            {
                // Use displayName instead of name to get a cleaner directory name
                string packagePath = Path.Combine(
                    "Packages",
                    package.packageId.Split('@')[0],
                    "Scenes"
                );
                string destinationPath = Path.Combine("Assets", "Scenes", package.displayName);

                // Only proceed if the Scenes directory exists in the package
                if (Directory.Exists(packagePath) && !Directory.Exists(destinationPath))
                {
                    CustomDialog.ConfirmationDialog(
                        title: $"Copy Scenes From New Package",
                        text: $"The package {package.displayName} has read-only scenes."
                            + $"Would you like to copy them to {destinationPath}",
                        onYes: () =>
                        {
                            CopyScenes(packagePath, destinationPath);
                        },
                        yesText: "Copy For Me",
                        noText: "I'll do it later"
                    );
                }
            }
        }

        private static void CopyScenes(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Copy all .unity files within the Scenes directory
            foreach (
                string filePath in Directory.GetFiles(
                    sourceDir,
                    "*.unity",
                    SearchOption.AllDirectories
                )
            )
            {
                string destFilePath = filePath.Replace(sourceDir, destinationDir);
                File.Copy(filePath, destFilePath, true);
            }
        }
    }
}
