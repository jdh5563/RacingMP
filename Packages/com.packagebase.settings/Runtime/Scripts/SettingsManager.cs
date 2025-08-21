using packageBase.audio;
using packageBase.core;
using packageBase.userInterfaces;
using UnityEngine;

namespace packageBase.settings
{
    public class SettingsManager : MonoBehaviour, ISettingsManager
    {
        private float _masterVolume;
        private float _sfxVolume;
        private float _musicVolume;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<SettingsManager>(this);
        }

        private void Start()
        {
            EventManager.Instance.SubscribeEvent(typeof(MenuSliderChangeEvent), this);
            EventManager.Instance.SubscribeEvent(typeof(PlaySoundEventStart), this);
        }

        public void OnEventHandler(in MenuSliderChangeEvent e)
        {
            float value = e.NewValue;

            if (value > 1.0f)
            {
                value = 1.0f;
            }
            else if (value < 0.0f)
            {
                value = 0.0f;
            }

            switch (e.SliderType)
            {
                case SliderTypes.SFXVolume:

                    _sfxVolume = value;

                    break;

                case SliderTypes.MusicVolume:

                    _musicVolume = value;

                    break;

                case SliderTypes.MasterVolume:

                    _masterVolume = value;

                    break;

                default:

                    Debug.LogWarning("Unhandled audio type passed in.");

                    break;
            }
        }

        public void OnEventHandler(in PlaySoundEventStart e)
        {
            float targetVolume = _masterVolume;

            switch (e.AudioType)
            {
                case AudioTypes.SFX:

                    targetVolume *= _sfxVolume;

                    break;

                case AudioTypes.Music:

                    targetVolume *= _musicVolume;

                    break;

                default:

                    Debug.LogWarning("An unhandled type of audio was passed in.");

                    break;
            }

            PlaySoundEventFinish playSoundEventFinish = new (e, targetVolume);
            EventManager.Instance.PublishEvent<PlaySoundEventFinish>(in playSoundEventFinish);
        }
    }
}
