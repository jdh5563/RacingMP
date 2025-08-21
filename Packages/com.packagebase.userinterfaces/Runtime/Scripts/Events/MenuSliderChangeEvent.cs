namespace packageBase.userInterfaces
{
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
