using UnityEngine.InputSystem;

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

    public struct MenuInputEvent
    {
        public InputAction.CallbackContext Context { get; private set; }

        public MenuInputEvent(InputAction.CallbackContext context)
        {
            Context = context;
        }
    }

    public struct MenuSliderChangeEvent
    {
        public float NewValue { get; private set; }

        public SliderTypes SliderType { get; private set; }

        public MenuSliderChangeEvent(float newValue, SliderTypes sliderType)
        {
            NewValue = newValue;
            SliderType = sliderType;
        }
    }
}