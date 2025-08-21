using UnityEngine.InputSystem;

namespace packageBase.input
{
    public struct MenuInputEvent
    {
        public InputAction.CallbackContext Context { get; private set; }

        public MenuInputEvent(InputAction.CallbackContext context)
        {
            Context = context;
        }
    }
}
