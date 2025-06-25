using packageBase.core;
using UnityEngine;

namespace packageBase.userInterfaces
{
    public class SettingsManager : InitableBase
    {
        private float _masterVolume;
        private float _sfxVolume;
        private float _musicVolume;

        public float MasterVolume
        {
            get { return _masterVolume; }
            set 
            {
                if (value > 1.0f)
                {
                    _masterVolume = 1.0f;
                }
                else if (value < 0.0f)
                {
                    _masterVolume = 0.0f;
                }
                else
                {
                    _masterVolume = value;
                }
            }
        }

        public float SFXVolume
        {
            get { return _sfxVolume; }
            set
            {
                if (value > 1.0f)
                {
                    _sfxVolume = 1.0f;
                }
                else if (value < 0.0f)
                {
                    _sfxVolume = 0.0f;
                }
                else
                {
                    _sfxVolume = value;
                }
            }
        }

        public float MusicVolume
        {
            get { return _musicVolume; }
            set
            {
                if (value > 1.0f)
                {
                    _musicVolume = 1.0f;
                }
                else if (value < 0.0f)
                {
                    _musicVolume = 0.0f;
                }
                else
                {
                    _musicVolume = value;
                }
            }
        }

        public override void DoInit()
        {
            base.DoInit();

            DontDestroyOnLoad(gameObject);

            ReferenceManager.Instance.AddReference<SettingsManager>(this);
        }
    }
}
