namespace MCG.UnityCheatSheet
{
    public static class PlatformService
    {
        public static bool IsUnityEditor =>
#if UNITY_EDITOR
            true;
#else
            false;
#endif
        public static bool IsAndroid =>
#if UNITY_ANDROID
            true;
#else
            false;
#endif
        public static bool IsWebGL =>
#if UNITY_WEBGL
            true;
#else
            false;
#endif
    }
}
