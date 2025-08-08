using packageBase.core;
using packageBase.eventManagement;
using System.Collections.Generic;
using UnityEngine;

namespace packageBase.userInterfaces
{
    public class SettingsMenu : Menu
    {
        private readonly List<MenuSlider> _sliders = new();

        public override void DoInit()
        {
            base.DoInit();

            for (int s = 0; s < transform.childCount; s++)
            {
                if (transform.GetChild(s).TryGetComponent(out MenuSlider slider))
                {
                    _sliders.Add(slider);
                }
            }
        }

        public override void DoPostInit()
        {
            base.DoPostInit();

            for (int s = 0; s < _sliders.Count; s++)
            {
                MenuSlider menuSlider = _sliders[s];
                _sliders[s].onValueChanged.AddListener((newValue) =>
                {
                    _slider_OnValueChanged(newValue, menuSlider);
                });
            }
        }

        /// <summary>
        /// Listener function attached to each slider on value changed event in this menu.
        /// </summary>
        /// <param name="newValue">The new value of the slider.</param>
        /// <param name="menuSlider">The slider that was adjusted.</param>
        private void _slider_OnValueChanged(float newValue, MenuSlider menuSlider)
        {
            SliderValueChangeEvent sliderValueChangeEvent = default;

            switch (menuSlider.SliderType)
            {
                case SliderTypes.MasterVolume:

                    sliderValueChangeEvent = new SliderValueChangeEvent(newValue, AudioTypes.Master);

                    break;

                case SliderTypes.SFXVolume:

                    sliderValueChangeEvent = new SliderValueChangeEvent(newValue, AudioTypes.SFX);

                    break;

                case SliderTypes.MusicVolume:

                    sliderValueChangeEvent = new SliderValueChangeEvent(newValue, AudioTypes.Music);

                    break;

                default:

                    Debug.LogWarning("An unhandled slider type was passed in.");

                    break;
            }

            EventManager.Instance.PublishEvent<SliderValueChangeEvent>(sliderValueChangeEvent);
        }
    }
}
