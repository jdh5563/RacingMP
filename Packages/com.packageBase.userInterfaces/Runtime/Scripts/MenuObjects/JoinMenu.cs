using packageBase.core;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace packageBase.userInterfaces
{
    public class JoinMenu : Menu
    {
        [SerializeField]
        private TMP_InputField _joinCodeInputField;

        [SerializeField]
        private TextMeshProUGUI _joinedPlayersText;

        private MultiplayerMenuManager _helloWorldManager;

        public override void DoPostInit()
        {
            base.DoPostInit();

            _helloWorldManager = ReferenceManager.Instance.GetReference<MultiplayerMenuManager>();

            if (_joinCodeInputField != null)
            {
                _joinCodeInputField.onSubmit.AddListener((inputValue) => { _joinCodeInputField_OnSubmit(inputValue); });
            }
        }

        public override void DoDestroy()
        {
            base.DoDestroy();

            if (_joinCodeInputField != null)
            {
                _joinCodeInputField.onSubmit.RemoveAllListeners();
            }
        }

        /// <summary>
        /// Async function used to attempt to join a server when the join code input field submit event occurs.
        /// </summary>
        /// <param name="inputValue">The value coming in from the input field.</param>
        private async void _joinCodeInputField_OnSubmit(string inputValue)
        {
            bool joinSuccessful = await _helloWorldManager.StartClientWithRelay(inputValue, "udp");

            if (!joinSuccessful)
            {
                Debug.LogWarning($"Joining server with join code {inputValue} failed.");
            }
            else
            {
                Debug.Log($"Joining server with join code {inputValue} was a success.");

                /*foreach (NetworkClient connectedPlayer in NetworkManager.Singleton.ConnectedClientsList)
                {
                    _joinedPlayersText.text += $"\nPlayer {connectedPlayer.ClientId}";
                }*/
            }
        }
    }
}
