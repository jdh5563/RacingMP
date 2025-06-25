using packageBase.core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Class responsible for handling changing menu states and other menu logic.
    /// </summary>
    public class MenuManager : InitableBase, IMenuManager
    {
        #region Fields

        private MenuInput _menuInput;
        private IGlobalInputManager _globalInputManager;
        private SettingsManager _settingsManager;

        private Menus _previousMenu = Menus.None;
        private List<Menu> _menuObjs = new List<Menu>();

        #endregion

        #region InitiableBase

        public override void DoInit()
        {
            base.DoInit();

            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<MenuManager>(this);

            for (int m = 0; m < transform.childCount; m++)
            {
                if (transform.GetChild(m).TryGetComponent(out Menu menuObject))
                {
                    _menuObjs.Add(menuObject);
                }
            }
        }

        public override void DoPostInit()
        {
            base.DoPostInit();

            _globalInputManager = ReferenceManager.Instance.GetReference<GlobalInputManager>();
            _menuInput = ReferenceManager.Instance.GetReference<MenuInput>();
            _settingsManager = ReferenceManager.Instance.GetReference<SettingsManager>();
        }

        public override void DoDestroy()
        {
            base.DoDestroy();

            ReferenceManager.Instance.RemoveReference<MenuManager>();
        }

        #endregion

        #region IMenuManager

        public Menus CurrentMenu { get; private set; } = Menus.MainMenu;

        public event EventHandler<MenuChangeArgs> OnMenuChange;

        public void HandleMenuInput(InputAction.CallbackContext context)
        {
            // No menu input should be tracked if not in a menu.
            if (CurrentMenu == Menus.None || CurrentMenu == Menus.LoadingScreen)
            {
                return;
            }

            // If the input action name can't be parsed to a menu input type enum, do nothing.
            if (!Enum.TryParse(context.action.name, out MenuInputTypes inputType))
            {
                return;
            }

            // Handling specific functionality for each type of menu input.
            switch (inputType)
            {
                case MenuInputTypes.Point:

                    break;

                case MenuInputTypes.LeftClick:

                    break;

                case MenuInputTypes.MiddleClick:

                    break;

                case MenuInputTypes.RightClick:

                    break;

                case MenuInputTypes.ScrollWheel:

                    // If the scroll is currently in cooldown, do nothing.
                    if (!_menuInput.CanScroll)
                    {
                        return;
                    }

                    // Creating a new axis event data object to store the move direction from the scroll wheel.
                    AxisEventData newData = new AxisEventData(EventSystem.current);
                    newData.moveDir = context.ReadValue<Vector2>().y > 0.0f ? MoveDirection.Up : MoveDirection.Down;

                    // Setting the currently selected object to move based on the axis event data above.
                    GameObject currentSelection = _globalInputManager.GetCurrentEventSystem().currentSelectedGameObject;
                    currentSelection.GetComponent<Selectable>().OnMove(newData);

                    // PLAY MOVE SOUND HERE.

                    StartCoroutine(_menuInput.ScrollCooldown());

                    break;

                case MenuInputTypes.Move:
                    // PLAY MOVE SOUND HERE.

                    break;

                case MenuInputTypes.Submit:

                    break;

                // Cancel input acts as a back button.
                case MenuInputTypes.Cancel:

                    // Don't allow cancel input to do anything if in any of these menus.
                    if (CurrentMenu == Menus.MainMenu || CurrentMenu == Menus.LoadingScreen)
                    {
                        return;
                    }

                    // PLAY CANCEL SOUND HERE.

                    toggleMenu(_previousMenu);

                    break;

                default:

                    Debug.LogWarning("An unhandled input was passed in.");
                    break;
            }
        }

        public void HandleMenuButtonClick(MenuButton menuButton)
        {
            // PLAY SUBMIT/CLICK SOUND HERE.

            // Handling functionality for each of the menu button types.
            switch (menuButton.MenuButtonType)
            {
                // Clicking the back button goes to the previous menu.
                case MenuButtonTypes.Back:

                    toggleMenu(_previousMenu);
                    break;

                // Clicking the credits button toggles the credits menu.
                case MenuButtonTypes.Credits:

                    toggleMenu(Menus.Credits);
                    break;

                // Clicking the settings button toggles the settings menu.
                case MenuButtonTypes.Settings:

                    toggleMenu(Menus.SettingsMenu);
                    break;

                // Clicking the start button toggles the loading screen.
                case MenuButtonTypes.Start:

                    toggleMenu(Menus.LoadingScreen);
                    break;

                // Clicking the resume button toggles the empty menu.
                case MenuButtonTypes.Resume:

                    toggleMenu(Menus.None);
                    break;

                // Clicking the quit button closes the application if in the main menu.
                // Clicking the quit button returns to the main menu if not in the main menu.
                case MenuButtonTypes.Quit:

                    if (CurrentMenu == Menus.MainMenu)
                    {
                        handleQuit();
                    }
                    else
                    {
                        toggleMenu(Menus.MainMenu);
                    }
                    break;

                // Handling any edge cases.
                default:

                    Debug.LogWarning("An unhandled button type was pressed.");
                    break;
            }
        }

        public void HandleSliderValueChange(float newValue, MenuSlider menuSlider)
        {
            switch (menuSlider.SliderType)
            {
                case SliderTypes.MasterVolume:

                    _settingsManager.MasterVolume = newValue;
                    break;

                case SliderTypes.SFXVolume:

                    _settingsManager.SFXVolume = newValue;
                    break;

                case SliderTypes.MusicVolume:

                    _settingsManager.MusicVolume = newValue;
                    break;

                default:

                    Debug.LogWarning("An unhandled slider type was modified.");
                    break;
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Helper function used to toggle a new menu on, and the previous one off.
        /// </summary>
        /// <param name="newMenu">The new menu being opened.</param>
        private void toggleMenu(Menus newMenu)
        {
            // Using a try-catch block to ensure nothing is null.
            try
            {
                _previousMenu = CurrentMenu;
                CurrentMenu = newMenu;

                Menu previousMenuObject = _menuObjs[(int)_previousMenu];
                Menu newMenuObject = _menuObjs[(int)newMenu];

                previousMenuObject.ToggleMenu();
                newMenuObject.ToggleMenu();

                // Checking that the last selected button of the new menu has been set.
                if (newMenuObject.LastSelectedButton != null)
                {
                    _globalInputManager.GetCurrentEventSystem().SetSelectedGameObject(newMenuObject.LastSelectedButton.gameObject);
                }
                else
                {
                    // If this returns null, that means there are no buttons in that menu.
                    if (newMenuObject.GetMenuButtonAtIndex(0) != null)
                    {
                        _globalInputManager.GetCurrentEventSystem().SetSelectedGameObject(newMenuObject.GetMenuButtonAtIndex(0).gameObject);
                    }
                }

                // When entering the menu, switch to the menu input action map.
                if (newMenu != Menus.None)
                {
                    _globalInputManager.ChangeCurrentInputMap(_menuInput.MenuMapName);
                }
                else
                {
                    _globalInputManager.ChangeCurrentInputMap("MainMap");
                }

                OnMenuChange?.Invoke(this, new MenuChangeArgs(_previousMenu, newMenu));
            }
            catch (Exception ex)
            {
                Debug.LogError($"{nameof(MenuManager)}: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper function used to handle quitting the application.
        /// </summary>
        private void handleQuit()
        {
            if (Application.isEditor)
            {
                EditorApplication.ExitPlaymode();
            }
            else
            {
                Application.Quit();
            }
        }

        #endregion
    }
}