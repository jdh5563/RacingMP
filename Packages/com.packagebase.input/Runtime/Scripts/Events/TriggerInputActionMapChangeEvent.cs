namespace packageBase.input
{
    public struct TriggerInputActionMapChangeEvent
    {
        public string NewInputActionMapName { get; private set; }

        public TriggerInputActionMapChangeEvent(string newInputActionMapName)
        {
            NewInputActionMapName = newInputActionMapName;
        }
    }
}
