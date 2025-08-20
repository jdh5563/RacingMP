using packageBase.core;
using TMPro;
using UnityEngine;
using Unity.Netcode;
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

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            _helloWorldManager = ReferenceManager.Instance.GetReference<MultiplayerMenuManager>();

			NetworkManager.Singleton.OnConnectionEvent += _networkManager_LobbyConnectionEvent;

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

                _joinCodeInputField.gameObject.SetActive(false);
            }
		}

		private void _networkManager_LobbyConnectionEvent(NetworkManager arg1, ConnectionEventData arg2)
		{
			if (arg2.EventType == ConnectionEvent.ClientConnected)
            {
				_joinedPlayersText.text = "Joined Players:";

				foreach (NetworkObject player in arg1.SpawnManager.PlayerObjects)
				{
					_joinedPlayersText.text += $"\n{player.name}";
				}

				arg1.SceneManager.OnLoadEventCompleted += _networkManager_MatchStartEvent;
			}
		}

        private void _networkManager_MatchStartEvent(string sceneName, LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
        {
            if (sceneName == "JohnScene")
            {
                SceneChangeEvent sceneChangeEvent = new(sceneName);
                EventManager.Instance.PublishEvent<SceneChangeEvent>(in sceneChangeEvent);
            }
        }
	}
}
