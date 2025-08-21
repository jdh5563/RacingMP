namespace packageBase.userInterfaces
{
    public struct MenuChangeEvent
    {
        public Menus PreviousMenu { get; private set; }
        public Menus NewMenu { get; private set; }

        public MenuChangeEvent(Menus previousMenu, Menus newMenu)
        {
            PreviousMenu = previousMenu;
            NewMenu = newMenu;
        }
    }
}
