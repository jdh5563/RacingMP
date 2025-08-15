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
    public class MenuInput : MonoBehaviour, ISystem
    {
        private IGlobalInputManager _globalInputManager;

        private List<InputAction> _menuInputActions = new();
        private readonly float _scrollCooldownTime = 0.2f;

        private bool _canScroll = true;

        private void Awake()
        {
            ReferenceManager.Instance.AddReference<MenuInput>(this);
        }

        private void Start()
        {
            _globalInputManager = ReferenceManager.Instance.GetReference<IGlobalInputManager>();

            if (_globalInputManager.GetInputActionAsset() != null)
            {
                InputActionMap menuInputActionMap = _globalInputManager.GetInputActionMap("MenuMap");

                // Populating the input action list with all the menu input actions and binding an event to each.
                for (int a = 0; a < menuInputActionMap.actions.Count; a++)
                {
                    _menuInputActions.Add(menuInputActionMap.actions[a]);
                    _menuInputActions[a].performed += _menuInput_Performed;
                }
            }
        }

        private void OnDestroy()
        {
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
            MenuInputEvent menuChangeEvent = new(context);

            if (context.action.name.Contains("Scroll"))
            {
                if (_canScroll)
                {
                    StartCoroutine(scrollCooldown());
                }
                else
                {
                    return;
                }
            }

            EventManager.Instance.PublishEvent<MenuInputEvent>(in menuChangeEvent);
        }

        private IEnumerator scrollCooldown()
        {
            _canScroll = false;
            yield return new WaitForSeconds(_scrollCooldownTime);
            _canScroll = true;
        }
    }
}