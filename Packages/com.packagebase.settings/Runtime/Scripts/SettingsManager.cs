using packageBase.core;
using packageBase.userInterfaces;
using UnityEngine;

namespace packageBase.settings
{
    public class SettingsManager : InitableBase, ISubscriber<MenuSliderChangeEvent>
    {
        public float MasterVolume { get; private set; }
        public float SFXVolume { get; private set; }
        public float MusicVolume { get; private set; }

        public override void DoInit()
        {
            base.DoInit();

            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<SettingsManager>(this);
        }

        public override void DoPostInit()
        {
            base.DoPostInit();

            EventManager.Instance.SubscribeEvent(typeof(MenuSliderChangeEvent), this);
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
    }
}
