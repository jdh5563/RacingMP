using packageBase.core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace racingMP.player
{
    public class PlayerInput : InitableBase
    {
        private IGlobalInputManager _globalInputManager;

        private readonly List<InputAction> _playerInputActions = new();

        public Vector2 MoveInput { get; private set; } = Vector2.zero;

        public override void DoInit()
        {
            base.DoInit();

            ReferenceManager.Instance.AddReference<PlayerInput>(this);
        }

        public override void DoPostInit()
        {
            base.DoPostInit();

            _globalInputManager = ReferenceManager.Instance.GetReference<GlobalInputManager>();

            if (_globalInputManager.GetInputActionAsset() != null)
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

        public override void DoDestroy()
        {
            base.DoDestroy();

            ReferenceManager.Instance.RemoveReference<PlayerInput>();
        }
    }
}
