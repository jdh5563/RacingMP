using packageBase.core;
using System.Collections.Generic;

namespace packageBase.userInterfaces
{
    public class SettingsMenu : Menu
    {
        private readonly List<MenuSlider> _sliders = new();

        protected override void Awake()
        {
            for (int s = 0; s < transform.childCount; s++)
            {
                if (transform.GetChild(s).TryGetComponent(out MenuSlider slider))
                {
                    _sliders.Add(slider);
                }
            }

            base.Awake();
        }

        protected override void Start()
        {
            for (int s = 0; s < _sliders.Count; s++)
            {
                MenuSlider menuSlider = _sliders[s];
                _sliders[s].onValueChanged.AddListener((newValue) =>
                {
                    _slider_OnValueChanged(newValue, menuSlider);
                });
            }

            base.Start();
        }

        /// <summary>
        /// Listener function attached to each slider on value changed event in this menu.
        /// </summary>
        /// <param name="newValue">The new value of the slider.</param>
        /// <param name="menuSlider">The slider that was adjusted.</param>
        private void _slider_OnValueChanged(float newValue, MenuSlider menuSlider)
        {
            MenuSliderChangeEvent menuSliderChangeEvent = new(newValue, menuSlider.SliderType);

            EventManager.Instance.PublishEvent<MenuSliderChangeEvent>(menuSliderChangeEvent);
        }
    }
}
