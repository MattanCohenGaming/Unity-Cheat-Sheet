using UnityEngine;

namespace MCG.UnityCheatSheet
{
    public class AndroidAppOpener : MonoBehaviour
    {
        [SerializeField]
        private string applicationId = "com.company.application"; // your target bundle id

        public void OpenApp()
        {
#if UNITY_ANDROID
            bool fail = false;
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

            AndroidJavaObject launchIntent = null;
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>(
                    "getLaunchIntentForPackage",
                    applicationId
                );
            }
            catch
            {
                fail = true;
            }

            if (fail)
            {
                Debug.Log("There is no app");
            }
            else //open the app
                ca.Call("startActivity", launchIntent);

            up.Dispose();
            ca.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();
            Application.Quit();
#else
            Debug.LogWarning(
                $"App opener called outside of an android build to open {applicationId}"
            );
#endif
        }
    }
}
