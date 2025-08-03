using packageBase.core;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace packageBase.userInterfaces
{
    public class HostMenu : Menu
    {
        [SerializeField]
        private TextMeshProUGUI _joinCodeText;

        [SerializeField]
        private TextMeshProUGUI _joinedPlayerText;

        private MultiplayerMenuManager _helloWorldManager;

        public override void DoPostInit()
        {
            base.DoPostInit();

            _helloWorldManager = ReferenceManager.Instance.GetReference<MultiplayerMenuManager>();

            NetworkManager.Singleton.OnConnectionEvent += _networkManagerSingleton_OnConnectionEvent;
        }

        public override void DoDestroy()
        {
            base.DoDestroy();
        }

        private void _networkManagerSingleton_OnConnectionEvent(NetworkManager arg1, ConnectionEventData arg2)
        {
            _joinedPlayerText.text += $"\nPlayer {arg2.ClientId}";

            if (arg1.IsHost)
            {
                _joinedPlayerText.text += " - Host";
            }
        }

        /// <summary>
        /// Async function used to attempt to host a server and display a join code.
        /// </summary>
        public async void HostServer()
        {
            string joinCode = await _helloWorldManager.StartHostWithRelay("udp");

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
