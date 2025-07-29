namespace packageBase.core
{
    public interface IReferenceManager
    {
        void AddReference<T>(InitableBase obj);

        void RemoveReference<T>();

        T GetReference<T>() where T : InitableBase;
    }
}
