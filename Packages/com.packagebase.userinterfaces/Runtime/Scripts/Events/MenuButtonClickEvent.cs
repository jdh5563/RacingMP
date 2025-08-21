namespace packageBase.userInterfaces
{
    public struct MenuButtonClickEvent
    {
        public MenuButton ClickedMenuButton { get; private set; }

        public MenuButtonClickEvent(MenuButton clickedMenuButton)
        {
            ClickedMenuButton = clickedMenuButton;
        }
    }
}
