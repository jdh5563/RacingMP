using UnityEngine.UI;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Class used to represent a menu button.
    /// Inherits from the Unity UI Button class.
    /// </summary>
    public class MenuButton : Button
    {
        public MenuButtonTypes MenuButtonType { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            if (name.Contains("Back"))
            {
                MenuButtonType = MenuButtonTypes.Back;
            }
            else if (name.Contains("Resume"))
            {
                MenuButtonType = MenuButtonTypes.Resume;
            }
            else if (name.Contains("Settings"))
            {
                MenuButtonType = MenuButtonTypes.Settings;
            }
            /*else if (name.Contains("Start"))
            {
                MenuButtonType = MenuButtonTypes.Start;
            }*/
            else if (name.Contains("Start"))
            {
                MenuButtonType = MenuButtonTypes.Host;
            }
            else if (name.Contains("HostServer"))
            {
                MenuButtonType = MenuButtonTypes.HostServer;
            }
            else if (name.Contains("Join"))
            {
                MenuButtonType = MenuButtonTypes.Join;
            }
            else if (name.Contains("Quit"))
            {
                MenuButtonType = MenuButtonTypes.Quit;
            }
            else if (name.Contains("Credits"))
            {
                MenuButtonType = MenuButtonTypes.Credits;
            }
        }
    }
}
