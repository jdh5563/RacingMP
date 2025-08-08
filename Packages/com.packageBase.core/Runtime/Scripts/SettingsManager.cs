using packageBase.eventManagement;
using UnityEngine;

namespace packageBase.core
{
    public class SettingsManager : InitableBase, ISubscriber<SliderValueChangeEvent>
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

            EventManager.Instance.SubscribeEvent(typeof(SliderValueChangeEvent), this);
        }

        public void OnEventHandler(in SliderValueChangeEvent e)
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

            switch (e.AudioType)
            {
                case AudioTypes.SFX:

                    SFXVolume = value;

                    break;

                case AudioTypes.Music:

                    MusicVolume = value;

                    break;

                case AudioTypes.Master:

                    MasterVolume = value;

                    break;

                default:

                    Debug.LogWarning("Unhandled audio type passed in.");

                    break;
            }
        }
    }
}
