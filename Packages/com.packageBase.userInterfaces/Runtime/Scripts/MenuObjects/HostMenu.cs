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

			_hostServer();

			base.Start();
        }

        public void BeginMatch()
        {
            //_joinedPlayerText.text += $"\nPlayer {arg2.ClientId}";

            /*if (arg1.IsHost)
            {
                _joinedPlayerText.text += " - Host";
            }*/

            NetworkManager.Singleton.SceneManager.LoadScene("JohnScene", LoadSceneMode.Single);

            ToggleMenu();
        }

        /// <summary>
        /// Async function used to attempt to host a server and display a join code.
        /// </summary>
        private async void _hostServer()
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
