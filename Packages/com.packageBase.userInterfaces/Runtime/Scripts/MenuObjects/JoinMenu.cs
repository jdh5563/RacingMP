using packageBase.core;
using TMPro;
using UnityEngine;

namespace packageBase.userInterfaces
{
    public class JoinMenu : Menu
    {
        [SerializeField]
        private TMP_InputField _joinCodeInputField;

        private HelloWorldManager _helloWorldManager;

        public override void DoPostInit()
        {
            base.DoPostInit();

            _helloWorldManager = ReferenceManager.Instance.GetReference<HelloWorldManager>();

            if (_joinCodeInputField != null)
            {
                _joinCodeInputField.onSubmit.AddListener((inputValue) => { _joinCodeInputField_OnSubmit(inputValue); });
            }
        }

        private async void _joinCodeInputField_OnSubmit(string inputValue)
        {
            bool joinSuccessful = await _helloWorldManager.StartClientWithRelay(inputValue, "udp");

            if (!joinSuccessful)
            {
                Debug.LogWarning($"Joining server with join code {inputValue} failed.");
            }
        }
    }
}
