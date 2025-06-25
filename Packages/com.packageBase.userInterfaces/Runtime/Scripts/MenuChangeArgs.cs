using System;

namespace packageBase.userInterfaces
{
    public class MenuChangeArgs : EventArgs
    {
        public Menus PreviousMenu;
        public Menus NewMenu;

        public MenuChangeArgs(Menus previousMenu, Menus newMenu)
        {
            PreviousMenu = previousMenu;
            NewMenu = newMenu;
        }
    }
}