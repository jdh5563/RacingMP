using packageBase.core;
using packageBase.input;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Interface used by the menu manager.
    /// Defines functionality necessary for the menu manager.
    /// </summary>
    public interface IMenuManager : ISystem, ISubscriber<MenuInputEvent>, ISubscriber<MenuButtonClickEvent>, ISubscriber<SceneChangeEvent>, ISubscriber<PlayerPauseInputEvent>
    {
        /// <summary>
        /// The current active menu.
        /// </summary>
        Menus CurrentMenu { get; }
    }
}
