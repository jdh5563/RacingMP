using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using packageBase.core;

namespace packageBase.userInterfaces
{
	public class MultiplayerMenuManager : InitableBase
	{
		[SerializeField]
		private int _maxConnectedPlayers;

        public override void DoInit()
        {
			base.DoInit();

			ReferenceManager.Instance.AddReference<MultiplayerMenuManager>(this);
        }

		/*void SubmitNewPosition()
		{
			if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
			{
				foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
				{
					NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
					HelloWorldPlayer player = playerObject.GetComponent<HelloWorldPlayer>();
					player.Move();
				}
			}
			else if (NetworkManager.Singleton.IsClient)
			{
				NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
				HelloWorldPlayer player = playerObject.GetComponent<HelloWorldPlayer>();
				player.Move();
			}
		}*/

		public async Task<string> StartHostWithRelay(string connectionType)
		{
			await UnityServices.InitializeAsync();
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
			}
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxConnectedPlayers);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
			string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

			Debug.Log($"The host created with join code: {joinCode}");
			return NetworkManager.Singleton.StartHost() ? joinCode : null;
		}

		public async Task<bool> StartClientWithRelay(string joinCode, string connectionType)
		{
			// If there is no join code, return false.
			if (string.IsNullOrEmpty(joinCode))
			{
				return false;
			}

			Debug.Log($"The client is trying to connect with join code: {joinCode}");

			await UnityServices.InitializeAsync();
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
			}

			JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
			return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
		}
	}
}
