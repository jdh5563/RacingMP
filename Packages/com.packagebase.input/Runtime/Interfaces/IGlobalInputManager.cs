using packageBase.core;
using UnityEngine.InputSystem;

namespace packageBase.input
{
    /// <summary>
    /// Interface used to control what can be accessed through the global input manager.
    /// </summary>
    public interface IGlobalInputManager : ISystem, ISubscriber<TriggerInputActionMapChangeEvent>
    {
        /// <summary>
        /// The currently set input map.
        /// </summary>
        InputActionMap CurrentInputMap { get; }

        /// <summary>
        /// The global input action asset for the project.
        /// </summary>
        InputActionAsset GlobalInputActionAsset { get; }

        /// <summary>
        /// Function to return an input action map from it's name.
        /// </summary>
        /// <param name="mapName">The name of the target input action map.</param>
        /// <returns>The input action map with the passed in name.</returns>
        InputActionMap GetInputActionMap(string mapName);
    }
}
