using System;
using UnityEngine.InputSystem;

namespace packageBase.core
{
    public class InputActionMapChangeArgs : EventArgs
    {
        public InputActionMap PreviousInputActionMap;
        public InputActionMap NextInputActionMap;

        public InputActionMapChangeArgs(InputActionMap previousInputActionMap, InputActionMap nextInputActionMap)
        {
            PreviousInputActionMap = previousInputActionMap;
            NextInputActionMap = nextInputActionMap;
        }
    }
}
