using packageBase.core;
using TMPro;
using UnityEngine;

namespace packageBase.userInterfaces
{
    public class JoinMenu : Menu
    {
        [SerializeField]
        private TMP_InputField _joinCodeInputField;

        [SerializeField]
        private TextMeshProUGUI _joinedPlayersText;

        private MultiplayerMenuManager _helloWorldManager;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            _helloWorldManager = ReferenceManager.Instance.GetReference<MultiplayerMenuManager>();

            if (_joinCodeInputField != null)
            {
                _joinCodeInputField.onSubmit.AddListener((inputValue) => { _joinCodeInputField_OnSubmit(inputValue); });
            }

            base.Start();
        }

        private void OnDestroy()
        {
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

                ToggleMenu();

                /*foreach (NetworkClient connectedPlayer in NetworkManager.Singleton.ConnectedClientsList)
                {
                    _joinedPlayersText.text += $"\nPlayer {connectedPlayer.ClientId}";
                }*/
            }
        }
    }
}
