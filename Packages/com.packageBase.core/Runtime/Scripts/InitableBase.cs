using System.Threading.Tasks;
using UnityEngine;

namespace packageBase.core
{
    /// <summary>
    /// Abstract class used for all objects that will be instantiated in the world.
    /// </summary>
    public abstract class InitableBase : MonoBehaviour, IInitableBase
    {
        public async virtual void DoInit()
        {
            Debug.Log("Init called from: " + GetType());

            await Task.Delay(200);
        }

        public virtual void DoPostInit()
        {
            Debug.Log("PostInit called from: " + GetType());
        }

        public virtual void DoDestroy()
        {
            Debug.Log("Destroy called from: " + GetType());
        }

        void Awake()
        {
            DoInit();
        }

        void Start()
        {
            DoPostInit();
        }

        private void OnDestroy()
        {
            DoDestroy();
        }
    }
}
