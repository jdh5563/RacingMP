using packageBase.audio;
using packageBase.core;
using packageBase.userInterfaces;
using UnityEngine;

namespace packageBase.settings
{
    public class SettingsManager : MonoBehaviour, ISystem, ISubscriber<MenuSliderChangeEvent>, ISubscriber<PlaySoundEventStart>
    {
        public float MasterVolume { get; private set; }
        public float SFXVolume { get; private set; }
        public float MusicVolume { get; private set; }

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

                    SFXVolume = value;

                    break;

                case SliderTypes.MusicVolume:

                    MusicVolume = value;

                    break;

                case SliderTypes.MasterVolume:

                    MasterVolume = value;

                    break;

                default:

                    Debug.LogWarning("Unhandled audio type passed in.");

                    break;
            }
        }

        public void OnEventHandler(in PlaySoundEventStart e)
        {
            float targetVolume = MasterVolume;

            switch (e.AudioType)
            {
                case AudioTypes.SFX:

                    targetVolume *= SFXVolume;

                    break;

                case AudioTypes.Music:

                    targetVolume *= MusicVolume;

                    break;

                default:

                    Debug.LogWarning("An unhandled type of audio was passed in.");

                    break;
            }

            PlaySoundEventFinal playSoundEventFinal = new (e, targetVolume);
            EventManager.Instance.PublishEvent<PlaySoundEventFinal>(in playSoundEventFinal);
        }
    }
}
