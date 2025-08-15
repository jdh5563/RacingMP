using UnityEngine;

namespace packageBase.audio
{
    public struct PlaySoundEvent
    {
        public AudioSource AudioSource { get; private set; }

        public AudioClip AudioClip { get; private set; }

        public AudioTypes AudioType { get; private set; }

        public PlaySoundEvent(AudioSource audioSource, AudioClip audioClip, AudioTypes audioType)
        {
            AudioSource = audioSource;
            AudioClip = audioClip;
            AudioType = audioType;
        }
    }
}
