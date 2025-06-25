using packageBase.core;
using System.Collections.Generic;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Class to represent a Menu Object in the project.
    /// </summary>
    public class Menu : InitableBase
    {
        #region Fields

        protected IMenuManager menuManager;

        protected List<MenuButton> menuButtons = new List<MenuButton>();

        public MenuButton LastSelectedButton { get; private set; }

        #endregion

        #region InitiableBase

        public override void DoInit()
        {
            base.DoInit();

            for (int b = 0; b < transform.childCount; b++)
            {
                if (transform.GetChild(b).TryGetComponent(out MenuButton menuButton))
                {
                    menuButtons.Add(menuButton);
                }
            }
        }

        public override void DoPostInit()
        {
            base.DoPostInit();

            menuManager = ReferenceManager.Instance.GetReference<MenuManager>();

            for (int b = 0; b < menuButtons.Count; b++)
            {
                MenuButton menuButton = menuButtons[b];
                menuButton.onClick.AddListener(() => 
                { 
                    _button_OnClick(menuButton);
                });
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Function used to toggle this menu on or off.
        /// </summary>
        public void ToggleMenu()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }

        /// <summary>
        /// Function used to get a button in this menu from a specfic index.
        /// </summary>
        /// <param name="index">The index of the button being used.</param>
        /// <returns>Returns the menu button object at the specified index.</returns>
        public MenuButton GetMenuButtonAtIndex(int index)
        {
            if (index >= 0 && index < menuButtons.Count)
            {
                return menuButtons[index];
            }

            return null;
        }

        #endregion

        #region Event Functions

        /// <summary>
        /// On click event bound to each menu button.
        /// </summary>
        /// <param name="menuButton">The button that was pressed.</param>
        private void _button_OnClick(MenuButton menuButton) 
        {
            LastSelectedButton = menuButton;
            menuManager.HandleMenuButtonClick(menuButton);
        }

        #endregion
    }
}
