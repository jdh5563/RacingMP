namespace packageBase.audio
{
    public struct PlaySoundEventFinish
    {
        public PlaySoundEventStart PlaySoundEventStart { get; private set; }

        public float Volume { get; private set; }

        public PlaySoundEventFinish(PlaySoundEventStart playSoundEventStart, float volume)
        {
            PlaySoundEventStart = playSoundEventStart;
            Volume = volume;
        }
    }
}
