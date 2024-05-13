using UnityEditor;
using UnityEngine;

namespace MCG.UnityCheatSheet.Editor
{
    public class BuildButtonWindow : EditorWindow
    {
        private static BuildTarget previousPlatform;
        public static string AndroidBuildPath =>
            "Builds/" + EditorPrefs.GetString("AndroidBuildPath") + ".apk";
        public static string PcBuildPath =>
            "Builds/" + EditorPrefs.GetString("PCBuildPath") + ".exe";

        [MenuItem("Build/Build for PC or Android")]
        private static void BuildForPCAndAndroidMenuItem()
        {
            BuildOptionsWindow window = EditorWindow.GetWindow<BuildOptionsWindow>("Build Options");
            window.Show();
        }

        [MenuItem("Build/Switch to PC platform")]
        private static void SwitchToPCPlatformMenuItem()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Confirmation",
                "Switch to PC platform?",
                "Yes",
                "No"
            );
            if (confirmed)
            {
                SwitchToPCPlatform();
            }
        }

        [MenuItem("Build/Switch to Android platform")]
        private static void SwitchToAndroidPlatformMenuItem()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Confirmation",
                "Switch to Android platform?",
                "Yes",
                "No"
            );
            if (confirmed)
            {
                SwitchToAndroidPlatform();
            }
        }

        public static void BuildForPCAndAndroid()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Confirmation",
                "Build for PC and Android? (This might take a while...)",
                "Yes",
                "Not Now"
            );
            if (confirmed)
            {
                BuildPlayerOptions pcBuildOptions = CreatePCBuildOptions();
                BuildPlayerOptions androidBuildOptions = CreateAndroidBuildOptions();

                BuildPipeline.BuildPlayer(pcBuildOptions);
                BuildPipeline.BuildPlayer(androidBuildOptions);

                // Return to PC platform if the previous platform was PC
                if (previousPlatform == BuildTarget.StandaloneWindows)
                {
                    SwitchToPCPlatform();
                }
            }
        }

        public static void BuildForAndroid()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Confirmation",
                "Build for Android? (This might take a while...)",
                "Yes",
                "Not Now"
            );
            if (confirmed)
            {
                BuildPlayerOptions androidBuildOptions = CreateAndroidBuildOptions();
                BuildPipeline.BuildPlayer(androidBuildOptions);

                // Return to PC platform if the previous platform was PC
                if (previousPlatform == BuildTarget.StandaloneWindows)
                {
                    SwitchToPCPlatform();
                }
            }
        }

        public static void BuildForPico()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Confirmation",
                "Build for Pico? (This might take a while...)\n\nBefore proceeding, make sure to set up the Player Settings for Pico VR.\n\n1. Open Player Settings (Edit -> Project Settings -> Player).\n2. Go to XR Plug-in Management.\n3. Enable the PICO VR Plugin.",
                "Yes",
                "Not Now"
            );
            if (confirmed)
            {
                BuildPlayerOptions androidBuildOptions = CreateAndroidBuildOptions();
                BuildPipeline.BuildPlayer(androidBuildOptions);

                // Return to PC platform if previous platform was PC
                if (previousPlatform == BuildTarget.StandaloneWindows)
                {
                    SwitchToPCPlatform();
                }
            }
        }

        public static void BuildForPC()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Confirmation",
                "Build for PC? (This might take a while...)",
                "Yes",
                "Not Now"
            );
            if (confirmed)
            {
                BuildPlayerOptions pcBuildOptions = CreatePCBuildOptions();
                BuildPipeline.BuildPlayer(pcBuildOptions);
            }
        }

        public static void ShowEditBuildPaths()
        {
            EditBuildPathsWindow window = EditorWindow.GetWindow<EditBuildPathsWindow>(
                "Edit Build Paths"
            );
            window.Show();
        }

        private static BuildPlayerOptions CreatePCBuildOptions()
        {
            string gameName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;

            BuildPlayerOptions pcBuildOptions = new BuildPlayerOptions();
            pcBuildOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(
                EditorBuildSettings.scenes
            );
            pcBuildOptions.locationPathName = PcBuildPath;
            pcBuildOptions.target = BuildTarget.StandaloneWindows;
            pcBuildOptions.options = BuildOptions.None;

            // Exclude Burst debug information folder from PC build
            pcBuildOptions.extraScriptingDefines = new[] { "ENABLE_BURST_DEBUG_INFORMATION=0" };

            return pcBuildOptions;
        }

        private static BuildPlayerOptions CreateAndroidBuildOptions()
        {
            string gameName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;

            BuildPlayerOptions androidBuildOptions = new BuildPlayerOptions();
            androidBuildOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(
                EditorBuildSettings.scenes
            );
            androidBuildOptions.locationPathName = AndroidBuildPath;
            androidBuildOptions.target = BuildTarget.Android;
            androidBuildOptions.options = BuildOptions.None;

            // Exclude Burst debug information folder from Android build
            androidBuildOptions.extraScriptingDefines = new[]
            {
                "ENABLE_BURST_DEBUG_INFORMATION=0"
            };

            return androidBuildOptions;
        }

        private static void SwitchToPCPlatform()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Standalone,
                BuildTarget.StandaloneWindows
            );
            previousPlatform = BuildTarget.StandaloneWindows;
        }

        private static void SwitchToAndroidPlatform()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android,
                BuildTarget.Android
            );
            previousPlatform = BuildTarget.Android;
        }
    }

    public class EditBuildPathsWindow : EditorWindow
    {
        private string androidBuildPath;
        private string pcBuildPath;
        bool showFullPaths = false;

        static string fullPathPrefix;

        private void OnEnable()
        {
            EditBuildPathsWindow window = (EditBuildPathsWindow)
                EditorWindow.GetWindow(typeof(EditBuildPathsWindow));
            fullPathPrefix = Application
                .dataPath
                .Substring(0, Application.dataPath.LastIndexOf("/"));

            // find paths without Builds/ before and with no file extensions
            androidBuildPath = EditorPrefs.GetString("AndroidBuildPath");
            pcBuildPath = EditorPrefs.GetString("PCBuildPath");
        }

        string AddPrefix(string path) => fullPathPrefix + "/Builds/" + path;

        string RemovePrefix(string path) =>
            path.Substring("/Builds/".Length + fullPathPrefix.Length);

        private void OnGUI()
        {
            // show toggle to show full paths
            showFullPaths = GUILayout.Toggle(showFullPaths, "Show Full Paths");

            // set the min size of the window
            minSize = new Vector2(showFullPaths ? 800 : 600, 150);

            GUILayout.Label("Edit Build Paths", EditorStyles.boldLabel);

            // Input field for Android build path
            GUILayout.BeginHorizontal();
            GUILayout.Label("Android Build Path:", GUILayout.Width(150));
            if (showFullPaths)
            {
                string androidPath = AddPrefix(androidBuildPath) + ".apk";
                androidBuildPath = RemovePrefix(GUILayout.TextField(androidPath));
                androidBuildPath = androidBuildPath.Substring(
                    0,
                    androidBuildPath.Length - ".apk".Length
                );
                Debug.Log($"android {androidBuildPath}");
            }
            else
            {
                androidBuildPath = GUILayout.TextField(androidBuildPath);
            }
            GUILayout.EndHorizontal();

            // Input field for PC build path
            GUILayout.BeginHorizontal();
            GUILayout.Label("PC Build Path:", GUILayout.Width(150));
            if (showFullPaths)
            {
                string pcPath = AddPrefix(pcBuildPath) + ".exe";
                pcBuildPath = RemovePrefix(GUILayout.TextField(pcPath));
                pcBuildPath = pcBuildPath.Substring(0, pcBuildPath.Length - ".exe".Length);
            }
            else
            {
                pcBuildPath = GUILayout.TextField(pcBuildPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            // Save button
            if (GUILayout.Button("Save", GUILayout.Height(30)))
            {
                SaveBuildPaths();
            }
        }

        private void SaveBuildPaths()
        {
            // Save the build paths to EditorPrefs
            EditorPrefs.SetString("AndroidBuildPath", androidBuildPath);
            EditorPrefs.SetString("PCBuildPath", pcBuildPath);
            Close();
        }
    }

    public class BuildOptionsWindow : EditorWindow
    {
        private const int buttonHeight = 40;
        private const int buttonMargin = 10;

        private void OnGUI()
        {
            GUILayout.Label("Build Options", EditorStyles.boldLabel);

            if (GUILayout.Button("Build for PC and Android", GUILayout.Height(buttonHeight)))
            {
                BuildButtonWindow.BuildForPCAndAndroid();
                Close();
            }

            GUILayout.Space(buttonMargin);

            if (GUILayout.Button("Build for Android", GUILayout.Height(buttonHeight)))
            {
                BuildButtonWindow.BuildForAndroid();
                Close();
            }

            GUILayout.Space(buttonMargin);

            if (GUILayout.Button("Build for Pico", GUILayout.Height(buttonHeight)))
            {
                BuildButtonWindow.BuildForPico();
                Close();
            }

            GUILayout.Space(buttonMargin);

            if (GUILayout.Button("Build for PC", GUILayout.Height(buttonHeight)))
            {
                BuildButtonWindow.BuildForPC();
                Close();
            }

            GUILayout.Space(buttonMargin);

            if (GUILayout.Button("Edit Build Paths", GUILayout.Height(buttonHeight)))
            {
                BuildButtonWindow.ShowEditBuildPaths();
                Close();
            }
        }
    }
}
