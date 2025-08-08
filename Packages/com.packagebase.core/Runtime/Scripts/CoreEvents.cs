namespace packageBase.core
{
    public struct SliderValueChangeEvent
    {
        public float NewValue;
        public AudioTypes AudioType;

        public SliderValueChangeEvent(float newValue, AudioTypes audioType)
        {
            AudioType = audioType;
            NewValue = newValue;
        }
    }
}