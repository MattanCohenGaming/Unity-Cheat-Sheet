using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MCG.UnityCheatSheet
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public bool muted { get; private set; }

        [SerializeField]
        AudioClip buttonClip;

        [SerializeField]
        AudioClip correctAnswerClip;

        [SerializeField]
        AudioClip wrongAnswerClip;
        AudioSource mySource;

        // Start is called before the first frame update
        void Start()
        {
            mySource = GetComponent<AudioSource>();
        }

        public bool MuteUnMute()
        {
            muted = !muted;
            return muted;
        }

        void PlayEffect(AudioClip clip)
        {
            if (muted)
                return;

            mySource.PlayOneShot(clip);
        }

        public void PlayButtonSoundEffect()
        {
            PlayEffect(buttonClip);
        }

        public void PlayCorrectAnswerEffect()
        {
            PlayEffect(correctAnswerClip);
        }

        public void PlayWrongAnswerEffect()
        {
            PlayEffect(wrongAnswerClip);
        }
    }
}
