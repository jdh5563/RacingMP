using packageBase.core;
using racingMP.track;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace packageBase.userInterfaces
{
	public class GameUIManager : NetworkBehaviour, ISubscriber<EventTrackGenerated>, ISubscriber<EventCheckpointHit>, ISubscriber<EventCarSpawn>
	{
		[SerializeField] private TextMeshProUGUI _countdownText;
		[SerializeField] private TextMeshProUGUI _checkpointText;
		[SerializeField] private TextMeshProUGUI _raceEndTimerText;
		[SerializeField] private Transform _roundEndPanel;
		[SerializeField] private Button _nextRoundButton;

		private List<TextMeshProUGUI> _playerPointsTexts = new();
		private Vector3 basePointsTextPosition = new Vector3(-75, 100, 0);
		private int pointsTextOffset = -75;

		// Make sure the total number of checkpoints is synchronized across all clients
		private NetworkVariable<int> totalCheckpoints = new();
		private int currentCheckpoints = 0;

		private Dictionary<ulong, int> playerFinishDict = new();
		private int numPlayersFinished = 0;

		private Coroutine raceEndTimer = null;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);

			EventManager.Instance.SubscribeEvent(typeof(EventTrackGenerated), this);
			EventManager.Instance.SubscribeEvent(typeof(EventCheckpointHit), this);
			EventManager.Instance.SubscribeEvent(typeof(EventCarSpawn), this);
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			if (IsServer)
			{
				NetworkManager.SceneManager.OnLoadEventCompleted += Countdown;
				NetworkManager.OnClientConnectedCallback += ClientConnected;
				_nextRoundButton.onClick.AddListener(() =>
				{
					List<ulong> keysToModify = new List<ulong>(playerFinishDict.Keys);
					foreach (ulong clientId in keysToModify)
					{
						playerFinishDict[clientId] = 0;
					}

					StartNextRoundRpc();

					StartCoroutine(StartRace());
				});

				foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
				{
					playerFinishDict.Add(clientId, 0);

					SendPlayerPointsTextRpc(clientId);
				}
			}
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void StartNextRoundRpc()
		{
			currentCheckpoints = 0;
			numPlayersFinished = 0;
			_roundEndPanel.gameObject.SetActive(false);
			_nextRoundButton.gameObject.SetActive(false);

			EventManager.Instance.PublishEvent(new EventResetLevel());

			_checkpointText.text = $"Checkpoints: {currentCheckpoints} / {totalCheckpoints.Value}";
		}

		/// <summary>
		/// Creates new text objects as clients join
		/// </summary>
		/// <param name="newClientId">ID for the new client</param>
		[Rpc(SendTo.ClientsAndHost)]
		private void SendPlayerPointsTextRpc(ulong newClientId)
		{
			int numPlayers = NetworkManager.Singleton.SpawnManager.PlayerObjects.Count;
			if (NetworkManager.Singleton.SpawnManager.PlayerObjects[(int)newClientId].IsOwner)
			{
				for (int i = 0; i < numPlayers; i++)
				{
					_playerPointsTexts.Add(Instantiate(new GameObject(), _roundEndPanel).AddComponent<TextMeshProUGUI>());
					_playerPointsTexts[^1].alignment = TextAlignmentOptions.MidlineLeft;
					_playerPointsTexts[^1].rectTransform.anchoredPosition = new Vector3(basePointsTextPosition.x, basePointsTextPosition.y + pointsTextOffset * i, basePointsTextPosition.z);
					_playerPointsTexts[^1].text = $"Player {i + 1}: 0";
					// There will be more functionality to make this not redundant like filling the new object with current info instead of defaults
				}
			}
			else
			{
				_playerPointsTexts.Add(Instantiate(new GameObject(), _roundEndPanel).AddComponent<TextMeshProUGUI>());
				_playerPointsTexts[^1].alignment = TextAlignmentOptions.MidlineLeft;
				_playerPointsTexts[^1].rectTransform.anchoredPosition = new Vector3(basePointsTextPosition.x, basePointsTextPosition.y + pointsTextOffset * (numPlayers - 1), basePointsTextPosition.z);
				_playerPointsTexts[^1].text = $"Player {numPlayers}: 0";
			}
		}

		private void ClientConnected(ulong clientId)
		{
			if (IsServer)
			{
				playerFinishDict.Add(clientId, 0);

				SendPlayerPointsTextRpc(clientId);
			}
		}

		/// <summary>
		/// When "GameUIScene" loads on all clients, we begin the countdown timer to start the race
		/// </summary>
		private void Countdown(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
		{
			if (sceneName == "GameUIScene") StartCoroutine(StartRace());
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void UpdateCountdownRpc(string text)
		{
			_countdownText.text = text;
        }

		[Rpc(SendTo.ClientsAndHost)]
		private void CountdownFinishedRpc()
		{
            // Need to publish this event in the call back to the clients and host to ensure everyone receives the event data.
            EventManager.Instance.PublishEvent(new EventRaceStarted());
        }

		/// <summary>
		/// Runs a 3-second countdown
		/// </summary>
		private IEnumerator StartRace()
		{
			yield return new WaitForSeconds(1);

			UpdateCountdownRpc("3");

			yield return new WaitForSeconds(1);

			UpdateCountdownRpc("2");

			yield return new WaitForSeconds(1);

			UpdateCountdownRpc("1");

			yield return new WaitForSeconds(1);

            UpdateCountdownRpc("");
			CountdownFinishedRpc();
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void DisplayRaceEndTimerTextRpc(string time)
		{
			_raceEndTimerText.text = time;
		}

		/// <summary>
		/// Should run server-side only, handles all things that occur at the end of a race
		/// </summary>
		private void EndRace()
		{
			if (!IsServer) return;

			foreach(ulong clientId in playerFinishDict.Keys)
			{
				DisplayPlayerPointsTextRpc(clientId, playerFinishDict[clientId]);
			}

			_nextRoundButton.gameObject.SetActive(true);
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void DisplayPlayerPointsTextRpc(ulong clientId, int points)
		{
			_roundEndPanel.gameObject.SetActive(true); // Maybe move this to somewhere else so it's only called once?

			TextMeshProUGUI textObj = _playerPointsTexts[(int)clientId];
			string[] pointsTextSplit = textObj.text.Split(':');
			textObj.text = $"{pointsTextSplit[0]}: {int.Parse(pointsTextSplit[1].Trim()) + points}";
		}

		/// <summary>
		/// Handles when a player finishes a race on the server side
		/// </summary>
		[Rpc(SendTo.Server)]
		private void RequestPlayerFinishRpc(ulong clientId)
		{
			// Prevent duplicate finishes
			if (playerFinishDict[clientId] > 0) return;

			// Only start the timer when the first player finishes
			if (numPlayersFinished == 0)
			{
				raceEndTimer = StartCoroutine(StartRaceEndTimer(10));
			}

			// Bare bones score calculation
			playerFinishDict[clientId] = playerFinishDict.Keys.Count - numPlayersFinished;
			numPlayersFinished++;

			// Race is over if everyone finished
			if (numPlayersFinished == playerFinishDict.Keys.Count)
			{
				StopCoroutine(raceEndTimer);
				DisplayRaceEndTimerTextRpc("");
				EndRace();
			}
		}

		/// <summary>
		/// Timer to end the race
		/// </summary>
		private IEnumerator StartRaceEndTimer(int time)
		{
			if (!IsServer) yield return null;

			for(int i = time; i > 0; i--)
			{
				DisplayRaceEndTimerTextRpc(i.ToString());
				yield return new WaitForSeconds(1);
			}

			DisplayRaceEndTimerTextRpc("");
			EndRace();

			yield return null;
		}

		/// <summary>
		/// Update checkpoint text
		/// </summary>
		public void OnEventHandler(in EventCheckpointHit e)
		{
			if (NetworkManager.Singleton.SpawnManager.PlayerObjects[(int)e.ClientId].IsOwner)
			{
				if (!e.IsFinish)
				{
					_checkpointText.text = $"Checkpoints: {++currentCheckpoints} / {totalCheckpoints.Value}";
				}
				else if (currentCheckpoints == totalCheckpoints.Value)
				{
					RequestPlayerFinishRpc(e.ClientId);
				}
			}
		}

		/// <summary>
		/// Set initial checkpoint text for anyone joining the lobby
		/// </summary>
		public void OnEventHandler(in EventCarSpawn e)
		{
			if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[e.NetObjId].IsOwner)
			{
				_checkpointText.text = $"Checkpoints: {currentCheckpoints} / {totalCheckpoints.Value}";
			}
		}

		/// <summary>
		/// Set initial checkpoint text for the host
		/// </summary>
		public void OnEventHandler(in EventTrackGenerated e)
		{
			totalCheckpoints.Value = e.TotalCheckpoints;
			_checkpointText.text = $"Checkpoints: {currentCheckpoints} / {totalCheckpoints.Value}";
		}
	}
}
