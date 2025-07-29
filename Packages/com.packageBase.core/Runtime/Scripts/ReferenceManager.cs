using System;
using System.Collections.Generic;
using UnityEngine;

namespace packageBase.core
{
    /// <summary>
    /// Class used to store references to different InitableBase objects in the project.
    /// </summary>
    public class ReferenceManager : InitableBase, IReferenceManager
    {
        #region Fields

        private readonly Dictionary<Type, InitableBase> _references = new();

        public static IReferenceManager Instance { get; private set; }

        #endregion

        #region InitableBase

        public override void DoInit()
        {
            if (Instance != null && (object)Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);

            base.DoInit();
        }

        #endregion

        #region IReferenceManager

        public void AddReference<T>(InitableBase obj)
        {
            if (_references.ContainsKey(typeof(T)))
            {
                Debug.LogWarning("Warning: Already have a reference to type " + typeof(T));
                return;
            }
            _references.Add(typeof(T), obj);
        }

        public void RemoveReference<T>()
        {
            if (_references.ContainsKey(typeof(T)))
            {
                _references.Remove(typeof(T));
            }
        }

        public T GetReference<T>() where T : InitableBase
        {
            if (_references[typeof(T)] != null)
            {
                return (T)_references[typeof(T)];
            }

            return default;
        }

        #endregion
    }
}
