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
		[SerializeField] 
		private TextMeshProUGUI _countdownText;
		[SerializeField]
		private TextMeshProUGUI _checkpointText;
		[SerializeField]
		private TextMeshProUGUI _raceEndTimerText;

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

		private void ClientConnected(ulong clientId)
		{
			ServerInitPlayerFinishDictRpc(NetworkManager.SpawnManager.PlayerObjects[^1].NetworkObjectId);
		}

		/// <summary>
		/// Initializes the dictionary containing player scores
		/// </summary>
		[Rpc(SendTo.Server)]
		private void ServerInitPlayerFinishDictRpc(ulong netObjId)
		{
			playerFinishDict.Add(netObjId, 0);
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

		/// <summary>
		/// Handles when a player finishes a race on the server side
		/// </summary>
		[Rpc(SendTo.Server)]
		private void RequestPlayerFinishRpc(ulong netObjId)
		{
			// Prevent duplicate finishes
			if (playerFinishDict[netObjId] > 0) return;

			// Only start the timer when the first player finishes
			if (numPlayersFinished == 0)
			{
				raceEndTimer = StartCoroutine(StartRaceEndTimer(10));
			}

			// Bare bones score calculation
			playerFinishDict[netObjId] = playerFinishDict.Keys.Count - numPlayersFinished;
			numPlayersFinished++;

			// Race is over if everyone finished
			if (numPlayersFinished == playerFinishDict.Keys.Count)
			{
				StopCoroutine(raceEndTimer);
				DisplayRaceEndTimerTextRpc("");
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

			yield return null;
		}

		/// <summary>
		/// Update checkpoint text
		/// </summary>
		public void OnEventHandler(in EventCheckpointHit e)
		{
			if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[e.NetObjId].IsOwner)
			{
				if (!e.IsFinish)
				{
					_checkpointText.text = $"Checkpoints: {++currentCheckpoints} / {totalCheckpoints.Value}";
				}
				else if (currentCheckpoints == totalCheckpoints.Value)
				{
					RequestPlayerFinishRpc(e.NetObjId);
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
