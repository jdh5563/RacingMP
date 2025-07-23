using UnityEngine;

namespace packageBase.core
{
    public interface IAudioManager
    {
        void PlaySound(AudioSource audioSource, AudioClip audioClip, AudioTypes audioType);
    }
}
