using packageBase.audio;
using packageBase.core;
using packageBase.input;
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
    public class MenuManager : MonoBehaviour, IMenuManager
    {
        #region Fields

        private Menus _previousMenu = Menus.None;
        private readonly List<Menu> _menuObjs = new();

        #endregion

        #region Unity Functions

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<IMenuManager>(this);

            for (int m = 0; m < transform.childCount; m++)
            {
                if (transform.GetChild(m).TryGetComponent(out Menu menuObject))
                {
                    _menuObjs.Add(menuObject);
                }
            }
        }

        private void Start()
        {
            EventManager.Instance.SubscribeEvent(typeof(MenuInputEvent), this);
            EventManager.Instance.SubscribeEvent(typeof(MenuButtonClickEvent), this);
            EventManager.Instance.SubscribeEvent(typeof(SceneChangeEvent), this);
        }

        private void OnDestroy()
        {
            ReferenceManager.Instance.RemoveReference<IMenuManager>();
        }

        #endregion

        #region IMenuManager

        public Menus CurrentMenu { get; private set; } = Menus.MainMenu;

        #endregion

        #region Helper Functions

        /// <summary>
        /// Function to handle keyboard/controller input in menus.
        /// </summary>
        /// <param name="context">Unity input system object containing input information.</param>
        private void handleMenuInput(InputAction.CallbackContext context)
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

                    Debug.Log("Scrolling");

                    // Creating a new axis event data object to store the move direction from the scroll wheel.
                    AxisEventData newData = new(EventSystem.current);
                    newData.moveDir = context.ReadValue<Vector2>().y > 0.0f ? MoveDirection.Up : MoveDirection.Down;

                    // Setting the currently selected object to move based on the axis event data above.
                    GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
                    currentSelection.GetComponent<Selectable>().OnMove(newData);

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

        private void handleMenuButtonClick(MenuButton menuButton)
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

                // Clicking the resume button toggles the empty menu.
                case MenuButtonTypes.Resume:

                    toggleMenu(Menus.None);
                    break;

                case MenuButtonTypes.Host:

                    toggleMenu(Menus.HostMenu);
                    break;

                case MenuButtonTypes.HostServer:

                    HostMenu hostMenu = (HostMenu)_menuObjs[(int)CurrentMenu];
                    hostMenu.BeginMatch();
                    break;

                case MenuButtonTypes.Join:

                    toggleMenu(Menus.JoinMenu);
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
                    EventSystem.current.SetSelectedGameObject(newMenuObject.LastSelectedButton.gameObject);
                }
                else
                {
                    // If this returns null, that means there are no buttons in that menu.
                    if (newMenuObject.GetMenuButtonAtIndex(0) != null)
                    {
                        EventSystem.current.SetSelectedGameObject(newMenuObject.GetMenuButtonAtIndex(0).gameObject);
                    }
                }

                // When entering the menu, switch to the menu input action map.
                TriggerInputActionMapChangeEvent triggerInputActionMapChangeEvent = default;

                if (newMenu != Menus.None)
                {
                    triggerInputActionMapChangeEvent = new TriggerInputActionMapChangeEvent("MenuMap");
                }
                else
                {
                    triggerInputActionMapChangeEvent = new TriggerInputActionMapChangeEvent("PlayerMap");
                }

                EventManager.Instance.PublishEvent<TriggerInputActionMapChangeEvent>(in triggerInputActionMapChangeEvent);

                MenuChangeEvent menuChangeEvent = new(_previousMenu, newMenu);
                EventManager.Instance.PublishEvent<MenuChangeEvent>(in menuChangeEvent);
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

        public void OnEventHandler(in MenuInputEvent e)
        {
            handleMenuInput(e.Context);
        }

        public void OnEventHandler(in MenuButtonClickEvent e)
        {
            handleMenuButtonClick(e.ClickedMenuButton);
        }

        public void OnEventHandler(in SceneChangeEvent e)
        {
            if (e.SceneName != "MainMenu")
            {
                toggleMenu(Menus.None);
            }
            else
            {
                toggleMenu(Menus.MainMenu);
            }
        }

        #endregion
    }
}