using packageBase.core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace packageBase.input
{
    public class PlayerInput : MonoBehaviour, ISystem
    {
        private IGlobalInputManager _globalInputManager;

        private readonly List<InputAction> _playerInputActions = new();

        public Vector2 MoveInput { get; private set; } = Vector2.zero;

        private void Awake()
        {
            ReferenceManager.Instance.AddReference<PlayerInput>(this);
        }

        private void Start()
        {
            _globalInputManager = ReferenceManager.Instance.GetReference<IGlobalInputManager>();

            if (_globalInputManager.GlobalInputActionAsset != null)
            {
                InputActionMap playerInputActionMap = _globalInputManager.GetInputActionMap("PlayerMap");

                // Populating the input action list with all the menu input actions and binding an event to each.
                for (int a = 0; a < playerInputActionMap.actions.Count; a++)
                {
                    _playerInputActions.Add(playerInputActionMap.actions[a]);

                    switch (_playerInputActions[a].name)
                    {
                        case "Move":
                            _playerInputActions[a].started += _handleMoveInput;
                            _playerInputActions[a].performed += _handleMoveInput;
                            _playerInputActions[a].canceled += _handleMoveInput;

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void _handleMoveInput(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnDestroy()
        {
            ReferenceManager.Instance.RemoveReference<PlayerInput>();
        }
    }
}
