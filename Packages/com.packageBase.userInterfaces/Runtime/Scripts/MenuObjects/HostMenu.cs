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

			NetworkManager.Singleton.OnConnectionEvent += LobbyConnectionEvent;

			hostServer();

			base.Start();
        }

        public void LobbyConnectionEvent(NetworkManager arg1, ConnectionEventData arg2)
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
            NetworkManager.Singleton.SceneManager.LoadScene("JohnScene", LoadSceneMode.Single);

            ToggleMenu();
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
        }
    }
}
