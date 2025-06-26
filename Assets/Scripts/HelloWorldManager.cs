using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

namespace HelloWorld
{
	public class HelloWorldManager : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField _joinCodeField;

		VisualElement rootVisualElement;
		Button hostButton;
		Button clientButton;
		Button serverButton;
		Button moveButton;
		Label statusLabel;
		string joinCode = "";

		void OnEnable()
		{
			UIDocument uiDocument = GetComponent<UIDocument>();
			rootVisualElement = uiDocument.rootVisualElement;

			hostButton = CreateButton("HostButton", "Host");
			clientButton = CreateButton("ClientButton", "Client");
			serverButton = CreateButton("ServerButton", "Server");
			moveButton = CreateButton("MoveButton", "Move");
			statusLabel = CreateLabel("StatusLabel", "Not Connected");

			rootVisualElement.Clear();
			rootVisualElement.Add(hostButton);
			rootVisualElement.Add(clientButton);
			rootVisualElement.Add(serverButton);
			rootVisualElement.Add(moveButton);
			rootVisualElement.Add(statusLabel);

			hostButton.clicked += OnHostButtonClicked;
			clientButton.clicked += OnClientButtonClicked;
			serverButton.clicked += OnServerButtonClicked;
			moveButton.clicked += SubmitNewPosition;
		}

		void Update()
		{
			UpdateUI();
		}

		void OnDisable()
		{
			hostButton.clicked -= OnHostButtonClicked;
			clientButton.clicked -= OnClientButtonClicked;
			serverButton.clicked -= OnServerButtonClicked;
			moveButton.clicked -= SubmitNewPosition;
		}

		async void OnHostButtonClicked() => joinCode = await StartHostWithRelay(2, "udp");

		async void OnClientButtonClicked() => await StartClientWithRelay(_joinCodeField.text, "udp");

		void OnServerButtonClicked() => NetworkManager.Singleton.StartServer();

		// Disclaimer: This is not the recommended way to create and stylize the UI elements, it is only utilized for the sake of simplicity.
		// The recommended way is to use UXML and USS. Please see this link for more information: https://docs.unity3d.com/Manual/UIE-USS.html
		private Button CreateButton(string name, string text)
		{
			Button button = new Button();
			button.name = name;
			button.text = text;
			button.style.width = 240;
			button.style.backgroundColor = Color.white;
			button.style.color = Color.black;
			button.style.unityFontStyleAndWeight = FontStyle.Bold;
			return button;
		}

		private Label CreateLabel(string name, string content)
		{
			Label label = new Label();
			label.name = name;
			label.text = content;
			label.style.color = Color.black;
			label.style.fontSize = 18;
			return label;
		}

		void UpdateUI()
		{
			if (NetworkManager.Singleton == null)
			{
				SetStartButtons(false);
				SetMoveButton(false);
				SetStatusText("NetworkManager not found");
				return;
			}

			if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
			{
				SetStartButtons(true);
				SetMoveButton(false);
				SetStatusText("Not connected");
			}
			else
			{
				SetStartButtons(false);
				SetMoveButton(true);
				UpdateStatusLabels();
			}
		}

		void SetStartButtons(bool state)
		{
			hostButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
			clientButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
			serverButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
		}

		void SetMoveButton(bool state)
		{
			moveButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
			if (state)
			{
				moveButton.text = NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change";
			}
		}

		void SetStatusText(string text) => statusLabel.text = text;

		void UpdateStatusLabels()
		{
			string mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
			string transport = "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
			string modeText = "Mode: " + mode;
			SetStatusText($"{transport}\n{modeText}");
		}

		void SubmitNewPosition()
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
		}

		public async Task<string> StartHostWithRelay(int maxConnections, string connectionType)
		{
			await UnityServices.InitializeAsync();
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
			}
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
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
