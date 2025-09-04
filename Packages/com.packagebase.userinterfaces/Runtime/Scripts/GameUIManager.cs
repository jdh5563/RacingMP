using packageBase.core;
using racingMP.track;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace packageBase.userInterfaces
{
	public class GameUIManager : NetworkBehaviour, ISubscriber<EventTrackGenerated>, ISubscriber<EventCheckpointHit>, ISubscriber<EventCarSpawn>
	{
		[SerializeField] private TextMeshProUGUI _countdownText;
		[SerializeField] private TextMeshProUGUI _checkpointText;
		[SerializeField] private TextMeshProUGUI _raceEndTimerText;
		[SerializeField] private Transform _roundEndPanel;

		private List<TextMeshProUGUI> _playerPointsTexts = new();

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
			}
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void SendPlayerPointsTextRpc(ulong newClientId, int numTexts)
		{
			if (NetworkManager.Singleton.SpawnManager.PlayerObjects[(int)newClientId].IsOwner)
			{
				for (int i = 0; i < numTexts; i++)
				{
					_playerPointsTexts.Add(Instantiate(new GameObject(), _roundEndPanel).AddComponent<TextMeshProUGUI>());
					_playerPointsTexts[^1].text = "0";
					// There will probably be more arguments in the function that will make this not redundant
				}
			}
			else
			{
				_playerPointsTexts.Add(Instantiate(new GameObject(), _roundEndPanel).AddComponent<TextMeshProUGUI>());
				_playerPointsTexts[^1].text = "0";
			}
		}

		private void ClientConnected(ulong clientId)
		{
			if (IsServer)
			{
				playerFinishDict.Add(clientId, 0);

				SendPlayerPointsTextRpc(clientId, _playerPointsTexts.Count + 1);
			}
		}

		/// <summary>
		/// When "JohnScene" loads, we begin the countdown timer to start the race
		/// </summary>
		private void Countdown(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
		{
			if (sceneName == "JohnScene") StartCoroutine(StartRace());
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
			_roundEndPanel.gameObject.SetActive(false);

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

		/// <summary>
		/// Set initial checkpoint text for the host
		/// </summary>
		public void OnEventHandler(in EventTrackGenerated e)
		{
			totalCheckpoints.Value = e.TotalCheckpoints;
			_checkpointText.text = $"Checkpoints: {currentCheckpoints} / {totalCheckpoints.Value}";
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void DisplayRaceEndTimerTextRpc(string time)
		{
			_raceEndTimerText.text = time;
		}

		private void EndRace()
		{
			foreach(ulong clientId in playerFinishDict.Keys)
			{
				DisplayPlayerPointsTextRpc(clientId, playerFinishDict[clientId]);
			}
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void DisplayPlayerPointsTextRpc(ulong clientId, int points)
		{
			_roundEndPanel.gameObject.SetActive(true); // Maybe move this to somewhere else so it's only called once?

			TextMeshProUGUI textObj = _playerPointsTexts[(int)clientId];
			textObj.text = $"{int.Parse(textObj.text) + points}";
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
	}
}
