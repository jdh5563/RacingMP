using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace packageBase.core
{
    /// <summary>
    /// Class used to handle and track input throughout the project.
    /// </summary>
    public class GlobalInputManager : MonoBehaviour, IGlobalInputManager
    {
        [SerializeField]
        private InputActionAsset _inputActionAsset;

        private PlayerInput _playerInput;
        private InputActionMap _currentMap;

        public event EventHandler<InputActionMapChangeArgs> OnInputActionMapChange;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<IGlobalInputManager>(this);
            
            _playerInput = GetComponent<PlayerInput>();

            if (_inputActionAsset != null && _playerInput != null)
            {
                _currentMap = GetInputActionMap(_playerInput.defaultActionMap);
            }
        }

        private void OnDestroy()
        {
            ReferenceManager.Instance.RemoveReference<IGlobalInputManager>();
        }

        public InputActionMap GetCurrentInputMap()
        {
            if ( _currentMap != null)
            {
                return _currentMap;
            }

            return null;
        }

        public InputActionAsset GetInputActionAsset() 
        {
            if (_inputActionAsset != null )
            {
                return _inputActionAsset;
            }

            return null;
        }

        public EventSystem GetCurrentEventSystem()
        {
            return EventSystem.current;
        }

        public InputActionMap GetInputActionMap(string mapName)
        {
            for (int a = 0; a < GetInputActionAsset().actionMaps.Count; a++)
            {
                InputActionMap currentActionMap = GetInputActionAsset().actionMaps[a];

                // If the menu action is map is found, set it's reference to that action map.
                if (currentActionMap.name == mapName)
                {
                    return currentActionMap;
                }
            }

            return null;
        }

        public void ChangeCurrentInputMap(string newInputActionMapName)
        {
            // If trying to switch to the same map, do nothing.
            if (_currentMap.name == newInputActionMapName)
            {
                return;
            }

            InputActionMap previousInputActionMap = _currentMap;
            _currentMap = GetInputActionMap(newInputActionMapName);

            OnInputActionMapChange?.Invoke(this, new InputActionMapChangeArgs(previousInputActionMap, _currentMap));
        }
    }
}
