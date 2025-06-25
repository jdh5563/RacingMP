using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace packageBase.core
{
    /// <summary>
    /// Interface used to control what can be accessed through the global input manager.
    /// </summary>
    public interface IGlobalInputManager
    {
        /// <summary>
        /// Event that can be bound to when an input action map changes.
        /// </summary>
        event EventHandler<InputActionMapChangeArgs> OnInputActionMapChange;

        /// <summary>
        /// Function to return the current input map.
        /// </summary>
        /// <returns>Returns the current input map.</returns>
        InputActionMap GetCurrentInputMap();

        /// <summary>
        /// Function to return the game's input action asset.
        /// </summary>
        /// <returns>Returns the game's input action asset.</returns>
        InputActionAsset GetInputActionAsset();

        /// <summary>
        /// Function used to get the current Event System being used.
        /// </summary>
        /// <returns>The curent event system.</returns>
        EventSystem GetCurrentEventSystem();

        /// <summary>
        /// Function to return an input action map from it's name.
        /// </summary>
        /// <param name="mapName">The name of the target input action map.</param>
        /// <returns>The input action map with the passed in name.</returns>
        InputActionMap GetInputActionMap(string mapName);

        /// <summary>
        /// Function used to change the current input map.
        /// </summary>
        /// <param name="newInputActionMapName">The name of the new input map.</param>
        void ChangeCurrentInputMap(string newInputActionMapName);
    }
}
