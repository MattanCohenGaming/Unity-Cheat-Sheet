using UnityEditor;
using UnityEngine;

namespace MCG.UnityCheatSheet.Editor
{
    [CustomEditor(typeof(SceneStarter))]
    public class SceneStarterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

            if (GUILayout.Button("Start Scene"))
                (target as SceneStarter).StartScene();
            if (GUILayout.Button("Stop Scene"))
                (target as SceneStarter).StopScene();
        }
    }
}
