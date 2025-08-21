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
		private TextMeshProUGUI _roundEndText;

		// Make sure the total number of checkpoints is synchronized across all clients
		private NetworkVariable<int> totalCheckpoints = new();
		private int currentCheckpoints = 0;

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

			if (IsHost) NetworkManager.SceneManager.OnLoadEventCompleted += Countdown;
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
		private void DisplayRoundEndTextRpc(ulong netObjId)
		{
			if(!NetworkManager.Singleton.SpawnManager.SpawnedObjects[netObjId].IsOwner)
			{
				_roundEndText.text = "You lose...";
			}
			else
			{
				_roundEndText.text = "You win!";
			}
		}

		[Rpc(SendTo.Server)]
		private void RequestRoundEndTextRpc(ulong netObjId)
		{
			DisplayRoundEndTextRpc(netObjId);
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
					RequestRoundEndTextRpc(e.NetObjId);
				}
			}
		}

		/// <summary>
		/// Set initial checkpoint text for anyone joining the lobby
		/// </summary>
		public void OnEventHandler(in EventCarSpawn e)
		{
			if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[e.NetObjId].IsOwner) _checkpointText.text = $"Checkpoints: {currentCheckpoints} / {totalCheckpoints.Value}";
		}
	}
}
