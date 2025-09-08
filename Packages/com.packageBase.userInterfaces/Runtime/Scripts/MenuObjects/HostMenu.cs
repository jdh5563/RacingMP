using packageBase.core;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace packageBase.userInterfaces
{
    public class HostMenu : Menu
    {
        [SerializeField]
        private TextMeshProUGUI _joinCodeText;

        [SerializeField]
        private TextMeshProUGUI _joinedPlayerText;

        private MultiplayerMenuManager _multiplayerMenuManager;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            _multiplayerMenuManager = ReferenceManager.Instance.GetReference<MultiplayerMenuManager>();

			NetworkManager.Singleton.OnConnectionEvent += _networkManager_LobbyConnectionEvent;

			hostServer();

			base.Start();
        }

		private void _networkManager_LobbyConnectionEvent(NetworkManager arg1, ConnectionEventData arg2)
        {
			if (arg2.EventType == ConnectionEvent.ClientConnected)
			{
				_joinedPlayerText.text = "Joined Players:";

				foreach (NetworkObject player in arg1.SpawnManager.PlayerObjects)
				{
					_joinedPlayerText.text += $"\n{player.name}";
				}
			}
		}

        public void BeginMatch()
        {
			NetworkManager.Singleton.SceneManager.LoadScene("JohnScene", LoadSceneMode.Additive);

			SceneChangeEvent sceneChangeEvent = new("JohnScene");
            EventManager.Instance.PublishEvent<SceneChangeEvent>(in sceneChangeEvent);
        }

        /// <summary>
        /// Async function used to attempt to host a server and display a join code.
        /// </summary>
        private async void hostServer()
        {
            string joinCode = await _multiplayerMenuManager.StartHostWithRelay("udp");

            // If an empty string is returned, the server failed to start.
            if (string.IsNullOrEmpty(joinCode))
            {

            }
            else
            {
                _joinCodeText.text = $"Join Code: {joinCode}";
            }

			NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoadEventCompleted;
		}

		private void SceneLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
		{
			if (sceneName == "JohnScene")
			{
				NetworkManager.Singleton.SceneManager.LoadScene("GameUIScene", LoadSceneMode.Additive);
			}
		}
	}
}
