using packageBase.core;
using System;
using UnityEngine.InputSystem;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Interface used by the menu manager.
    /// Defines functionality necessary for the menu manager.
    /// </summary>
    public interface IMenuManager : ISystem
    {
        /// <summary>
        /// The current active menu.
        /// </summary>
        Menus CurrentMenu { get; }

        /// <summary>
        /// Function to handle when a menu button is clicked/selected.
        /// </summary>
        /// <param name="menuButton">The button that was pressed.</param>
        void HandleMenuButtonClick(MenuButton menuButton);
    }
}
