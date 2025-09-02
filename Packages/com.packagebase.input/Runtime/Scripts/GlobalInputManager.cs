using packageBase.core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace packageBase.input
{
    /// <summary>
    /// Class used to handle and track input throughout the project.
    /// </summary>
    public class GlobalInputManager : MonoBehaviour, IGlobalInputManager
    {
        [SerializeField]
        private InputActionAsset _globalInputActionAsset;

        public InputActionAsset GlobalInputActionAsset => _globalInputActionAsset;

        public InputActionMap CurrentInputMap { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<IGlobalInputManager>(this);

            if (_globalInputActionAsset != null && _globalInputActionAsset.actionMaps.Count > 0)
            {
                CurrentInputMap = GlobalInputActionAsset.actionMaps[0];
            }
        }

        private void Start()
        {
            EventManager.Instance.SubscribeEvent(typeof(TriggerInputActionMapChangeEvent), this);
        }

        private void OnDestroy()
        {
            ReferenceManager.Instance.RemoveReference<IGlobalInputManager>();
        }

        public InputActionMap GetInputActionMap(string mapName)
        {
            for (int a = 0; a < GlobalInputActionAsset.actionMaps.Count; a++)
            {
                InputActionMap currentActionMap = GlobalInputActionAsset.actionMaps[a];

                // If the menu action is map is found, set it's reference to that action map.
                if (currentActionMap.name == mapName)
                {
                    return currentActionMap;
                }
            }

            return null;
        }

        public void OnEventHandler(in TriggerInputActionMapChangeEvent e)
        {
            CurrentInputMap = GetInputActionMap(e.NewInputActionMapName);
        }
    }
}
