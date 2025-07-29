using packageBase.core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Class responsible for tracking and handling menu input.
    /// </summary>
    public class MenuInput : InitableBase
    {
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _menuNavigationAudioClip;

        private AudioManager _audioManager;
        private IGlobalInputManager _globalInputManager;
        private IMenuManager _menuManager;

        private List<InputAction> _menuInputActions = new();
        private readonly float _scrollCooldownTime = 0.2f;

        public bool CanScroll { get; private set; } = true;
        public string MenuMapName { get; private set; } = "MenuMap";

        public override void DoInit()
        {
            base.DoInit();

            ReferenceManager.Instance.AddReference<MenuInput>(this);
        }

        public override void DoPostInit()
        {
            base.DoPostInit();

            _audioManager = ReferenceManager.Instance.GetReference<AudioManager>();
            _menuManager = ReferenceManager.Instance.GetReference<MenuManager>();
            _globalInputManager = ReferenceManager.Instance.GetReference<GlobalInputManager>();

            if (_globalInputManager.GetInputActionAsset() != null)
            {
                InputActionMap menuInputActionMap = _globalInputManager.GetInputActionMap(MenuMapName);

                // Populating the input action list with all the menu input actions and binding an event to each.
                for (int a = 0; a < menuInputActionMap.actions.Count; a++)
                {
                    _menuInputActions.Add(menuInputActionMap.actions[a]);
                    _menuInputActions[a].performed += _menuInput_Performed;
                }
            }
        }

        public override void DoDestroy()
        {
            base.DoDestroy();

            // Removing the bound events from each menu input action.
            for (int a = 0; a < _menuInputActions.Count; a++)
            {
                _menuInputActions[a].performed -= _menuInput_Performed;
            }

            ReferenceManager.Instance.RemoveReference<MenuInput>();
        }

        /// <summary>
        /// Function that handles what happens with each different menu input action.
        /// </summary>
        /// <param name="context">Data fed in from the Unity Input system.</param>
        private void _menuInput_Performed(InputAction.CallbackContext context)
        {
            _menuManager.HandleMenuInput(context);
        }

        public IEnumerator ScrollCooldown()
        {
            CanScroll = false;
            yield return new WaitForSeconds(_scrollCooldownTime);
            CanScroll = true;
        }
    }
}