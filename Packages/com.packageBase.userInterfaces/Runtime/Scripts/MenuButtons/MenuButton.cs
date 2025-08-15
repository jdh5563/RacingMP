using UnityEngine;
using UnityEngine.UI;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Class used to represent a menu button.
    /// Inherits from the Unity UI Button class.
    /// </summary>
    public class MenuButton : Button
    {
        [SerializeField]
        private MenuButtonTypes _menuButtonType;

        public MenuButtonTypes MenuButtonType
        {
            get { return _menuButtonType; }
        }
    }
}
