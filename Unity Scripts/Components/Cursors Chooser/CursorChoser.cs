using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MCG.UnityCheatSheet
{
    public class CursorChoser : MonoBehaviour
    {
        public static CursorChoser instance;

        [SerializeField]
        Texture2D[] cursors;

        void Start()
        {
            if (instance != null)
                Destroy(gameObject);
            instance = this;

            DontDestroyOnLoad(gameObject);

            PickNewCursor();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PickNewCursor();
        }

        void PickNewCursor()
        {
            if (cursors.Length > 0)
                Cursor.SetCursor(
                    cursors[Random.Range(0, cursors.Length)],
                    Vector2.zero,
                    CursorMode.Auto
                );
        }
    }
}
