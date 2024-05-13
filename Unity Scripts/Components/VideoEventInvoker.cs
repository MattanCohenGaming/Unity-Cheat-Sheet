using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace MCG.UnityCheatSheet
{
    public class VideoEventInvoker : MonoBehaviour
    {
        [SerializeField]
        private bool isShowingVideo = true;

        [SerializeField]
        private VideoPlayer player;

        [Space, SerializeField]
        private UnityEvent beforeVideoEvent;

        [Space, SerializeField]
        private UnityEvent afterVideoEvent;

        private void Awake()
        {
            player.started += (player) => OnPlayVideo();
        }

        private void OnPlayVideo()
        {
            InvokeEvents(beforeVideoEvent);
            var videoTime = player.clip.length;
            InvokeEvents(afterVideoEvent, isShowingVideo ? videoTime : 0f);
        }

        private void InvokeEvents(UnityEvent unityEvent, double after = 0)
        {
            StartCoroutine(InvokeEventsCoroutine(unityEvent, after));
        }

        private IEnumerator InvokeEventsCoroutine(UnityEvent unityEvent, double after = 0)
        {
            yield return new WaitForSecondsRealtime((float)after);
            unityEvent?.Invoke();
        }
    }
}
