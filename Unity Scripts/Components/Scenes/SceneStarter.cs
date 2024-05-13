using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MCG.UnityCheatSheet
{
    /// <summary>
    /// A class to start a scene by code.
    /// every scene is built of 2 objects, the scene items and the scene starter.
    /// this class performs instance per scene ; every scene will forcefully have 1 scene starter
    ///
    /// public static functions:
    ///     ~~ SceneStarter.GetSceneStarterFor(Scene scene)
    ///         => returns the scene's SceneStarter
    ///
    /// public functions:
    ///     ~~ StartScene()
    ///         => starts the scene (enables the scene items)
    ///     ~~ StopScene()
    ///         => stops the scene (disables the scene items)
    /// </summary>
    public class SceneStarter : MonoBehaviour
    {
        static string gameObjectName = "Scene Starter";

        [Tooltip("The items to enable/disable on starting and stopping the scene")]
        [SerializeField]
        private GameObject sceneItems;

        [Space]
        [Tooltip("If true, will enable sceneItems on start. If false, will disable the sceneItems")]
        [SerializeField]
        private bool startSceneOnStart = false;

        [Space]
        [Tooltip("Those events will be invoked on starting the scene")]
        [SerializeField]
        private UnityEvent eventsOnStartingScene;

        #region instance functions
        private void Awake()
        {
            // changes the name of the object to not be the same as gameObjectName
            gameObject.name = "~ " + gameObjectName;
        }

        private void Start()
        {
            // Destroy if already exists in this scene (similiar to instance but for different scenes)
            if (GetSceneStarterFor(gameObject.scene) != null)
                Destroy(gameObject);
            // If passed, this object will be the instance (similiar to instance = this)
            gameObject.name = gameObjectName;

            if (startSceneOnStart)
                StartScene();
            else
                StopScene();
        }
        #endregion

        #region public functions
        public void StartScene()
        {
            sceneItems.SetActive(true);

            eventsOnStartingScene?.Invoke();
        }

        public void StopScene()
        {
            sceneItems.SetActive(false);
        }

        /// <summary>
        /// returns the instance of SceneStart in the input scene
        /// </summary>
        /// <param name="scene"> the scene to find its SceneStarter </param>
        /// <returns> that specific scene's SceneStarter </returns>
        public static SceneStarter GetSceneStarterFor(Scene scene)
        {
            SceneStarter sceneStarter = null;
            if (scene == null)
                return sceneStarter;

            GameObject specificObject = FindObjectInScene(scene, gameObjectName);
            if (specificObject == null)
                return sceneStarter;
            return specificObject.GetComponent<SceneStarter>();
        }
        #endregion

        #region  Helping Functions
        private static GameObject FindObjectInScene(Scene scene, string objectName)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject rootObject in rootObjects)
            {
                if (rootObject.name == objectName)
                {
                    return rootObject;
                }
            }

            return null;
        }
        #endregion

        #region editor helpers
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartScene();
        }
#endif
        #endregion
    }
}
