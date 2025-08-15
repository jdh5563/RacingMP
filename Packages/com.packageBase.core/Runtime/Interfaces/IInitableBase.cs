namespace packageBase.core
{
    /// <summary>
    /// Interface for all objects that inherit from MonoBehavior.
    /// </summary>
    public interface IInitableBase :  ISystem
    {
        /// <summary>
        /// Function run on the initialization of this script.
        /// </summary>
        void DoInit();
        /// <summary>
        /// Function run after the initialization of this script.
        /// </summary>
        void DoPostInit();
        /// <summary>
        /// Function run after the desctruction of this script.
        /// </summary>
        void DoDestroy();
    }
}
