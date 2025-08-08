using UnityEngine;

namespace packageBase.core
{
    public class AudioManager : InitableBase, IAudioManager
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

            _settingsManager = ReferenceManager.Instance.GetReference<SettingsManager>();
        }

        public override void DoDestroy()
        {
            base.DoDestroy();

            ReferenceManager.Instance.RemoveReference<AudioManager>();
        }

        public void PlaySound(AudioSource audioSource, AudioClip audioClip, AudioTypes audioType)
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
    }
}
