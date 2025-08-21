namespace packageBase.core
{
    public struct SceneChangeEvent
    {
        public string SceneName { get; private set; }

        public SceneChangeEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}
