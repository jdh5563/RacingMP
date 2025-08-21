using packageBase.core;
using UnityEngine;

namespace packageBase.audio
{
    public class AudioManager : MonoBehaviour, ISystem, ISubscriber<PlaySoundEventFinish>
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<AudioManager>(this);
        }

        private void Start()
        {
            EventManager.Instance.SubscribeEvent(typeof(PlaySoundEventFinish), this);
        }

        private void OnDestroy()
        {
            ReferenceManager.Instance.RemoveReference<AudioManager>();
        }

        private void PlaySound(AudioSource audioSource, AudioClip audioClip, float volume)
        {
            audioSource.volume = volume;

            audioSource.PlayOneShot(audioClip);
        }

        public void OnEventHandler(in PlaySoundEventFinish e)
        {
            PlaySound(e.PlaySoundEventStart.AudioSource, e.PlaySoundEventStart.AudioClip, e.Volume);
        }
    }
}
