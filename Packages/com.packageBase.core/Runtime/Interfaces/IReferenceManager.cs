namespace packageBase.core
{
    public interface IReferenceManager
    {
        void AddReference<T>(ISystem obj);

        void RemoveReference<T>();

        T GetReference<T>() where T : ISystem;
    }
}
