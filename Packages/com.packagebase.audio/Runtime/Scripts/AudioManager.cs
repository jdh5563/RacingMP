using packageBase.core;
using packageBase.settings;
using UnityEngine;

namespace packageBase.audio
{
    public class AudioManager : InitableBase, ISubscriber<PlaySoundEvent>
    {
        private SettingsManager _settingsManager;

        public override void DoInit()
        {
            base.DoInit();

            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<AudioManager>(this);
        }

        public override void DoPostInit()
        {
            base.DoPostInit();
        }

        public override void DoDestroy()
        {
            base.DoDestroy();

            ReferenceManager.Instance.RemoveReference<AudioManager>();
        }

        private void PlaySound(AudioSource audioSource, AudioClip audioClip, AudioTypes audioType)
        {
            switch (audioType) 
            {
                case AudioTypes.SFX:

                    audioSource.volume = _settingsManager.SFXVolume * _settingsManager.MasterVolume;
                    break;

                case AudioTypes.Music:

                    audioSource.volume = _settingsManager.MusicVolume * _settingsManager.MasterVolume;
                    break;

                default:

                    Debug.LogWarning("An unhandled audio type was passed in.");
                    break;
            }

            audioSource.PlayOneShot(audioClip);
        }

        public void OnEventHandler(in PlaySoundEvent e)
        {
            PlaySound(e.AudioSource, e.AudioClip, e.AudioType);
        }
    }
}
