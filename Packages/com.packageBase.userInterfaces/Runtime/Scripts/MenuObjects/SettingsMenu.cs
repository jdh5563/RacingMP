using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace packageBase.userInterfaces
{
    public class SettingsMenu : Menu
    {
        private List<MenuSlider> _sliders = new List<MenuSlider>();

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
            menuManager.HandleSliderValueChange(newValue, menuSlider);
        }
    }
}
