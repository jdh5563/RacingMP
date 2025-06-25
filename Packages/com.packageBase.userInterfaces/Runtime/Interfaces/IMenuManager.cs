using System;
using UnityEngine.InputSystem;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Interface used by the menu manager.
    /// Defines functionality necessary for the menu manager.
    /// </summary>
    public interface IMenuManager
    {
        /// <summary>
        /// The current active menu.
        /// </summary>
        Menus CurrentMenu { get; }

        /// <summary>
        /// Event that can be hooked up to when a menu changes.
        /// </summary>
        event EventHandler<MenuChangeArgs> OnMenuChange;

        /// <summary>
        /// Function to handle keyboard/controller input in menus.
        /// </summary>
        /// <param name="context">Unity input system object containing input information.</param>
        void HandleMenuInput(InputAction.CallbackContext context);

        /// <summary>
        /// Function to handle when a menu button is clicked/selected.
        /// </summary>
        /// <param name="menuButton">The button that was pressed.</param>
        void HandleMenuButtonClick(MenuButton menuButton);

        /// <summary>
        /// Function to handle when a menu slider value is changed.
        /// </summary>
        /// <param name="newValue">The new value of the slider.</param>
        /// <param name="menuSlider">The slider that was adjusted.</param>
        void HandleSliderValueChange(float newValue, MenuSlider menuSlider);
    }
}
