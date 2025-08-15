using UnityEngine;

namespace packageBase.audio
{
    public struct PlaySoundEventStart
    {
        public AudioSource AudioSource { get; private set; }

        public AudioClip AudioClip { get; private set; }

        public AudioTypes AudioType { get; private set; }

        public PlaySoundEventStart(AudioSource audioSource, AudioClip audioClip, AudioTypes audioType)
        {
            AudioSource = audioSource;
            AudioClip = audioClip;
            AudioType = audioType;
        }
    }

    public struct PlaySoundEventFinal
    {
        public PlaySoundEventStart PlaySoundEventStart { get; private set; }

        public float Volume { get; private set; }

        public PlaySoundEventFinal(PlaySoundEventStart playSoundEventStart, float volume)
        {
            PlaySoundEventStart = playSoundEventStart;
            Volume = volume;
        }
    }
}
