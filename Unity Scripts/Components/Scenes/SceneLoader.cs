using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MCG.UnityCheatSheet
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField, Tooltip("If true, will load the scene on awake")]
        private bool loadSceneOnAwake = false;

        [SerializeField, Tooltip("If true, will start the scene as soon as it loads.")]
        private bool startSceneOnLoad = true;

        [
            SerializeField,
            Tooltip("If true, will unload the current scene when loading is finished.")
        ]
        public bool unloadSceneWhenFinishedLoading = true;

        [
            SerializeField,
            Tooltip("If true, will unload the current scene when starting the scene to load.")
        ]
        public bool unloadSceneWhenStartingScene = true;

        [HideInInspector]
        public string sceneToLoadName;

        private void Awake()
        {
            if (loadSceneOnAwake)
            {
                LoadScene();
            }
        }

        public bool IsDesiredSceneLoaded => SceneManager.GetSceneByName(sceneToLoadName).isLoaded;

        /// <summary>
        /// Starts the scene by activating the SceneStarter in the loaded scene.
        /// </summary>
        public void StartScene()
        {
            // Ensure the scene is loaded before trying to find the SceneStarter.
            if (IsDesiredSceneLoaded)
            {
                SceneStarter sceneStarter = SceneStarter.GetSceneStarterFor(
                    SceneManager.GetSceneByName(sceneToLoadName)
                );
                if (sceneStarter != null)
                {
                    sceneStarter.StartScene();

                    if (unloadSceneWhenStartingScene)
                        SceneManager.UnloadSceneAsync(gameObject.scene);
                }
                else
                {
                    Debug.LogWarning("SceneStarter not found in the loaded scene.");
                }
            }
            else
            {
                Debug.LogWarning("Scene is not loaded. Cannot start the scene.");
            }
        }

        /// <summary>
        /// Loads the specified scene asynchronously.
        /// If startSceneOnLoad is true, starts the scene and unloads the current one.
        /// </summary>
        public void LoadScene()
        {
            bool reloadSameScene = sceneToLoadName == SceneManager.GetActiveScene().name;

            if (reloadSameScene)
                LoadSceneSync();
            else
                StartCoroutine(LoadSceneAsync());

            if (startSceneOnLoad)
            {
                StartScene();
            }
        }

        private void LoadSceneSync() => SceneManager.LoadScene(sceneToLoadName);

        private IEnumerator LoadSceneAsync()
        {
            yield return SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive);
            if (unloadSceneWhenFinishedLoading)
                yield return SceneManager.UnloadSceneAsync(gameObject.scene);
        }
    }
}
