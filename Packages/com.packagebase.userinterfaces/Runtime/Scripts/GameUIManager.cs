using packageBase.core;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using TMPro;

namespace packageBase.userInterfaces
{
	public class GameUIManager : NetworkBehaviour
	{
		[SerializeField] private TextMeshProUGUI countdownText;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public override void OnNetworkSpawn()
		{
			if (IsHost) NetworkManager.SceneManager.OnLoadEventCompleted += Countdown;
		}

		/// <summary>
		/// When "JohnScene" loads, we begin the countdown timer to start the race
		/// </summary>
		private void Countdown(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
		{
			if (sceneName == "JohnScene") StartCoroutine(StartRace());
		}

		[Rpc(SendTo.Server)]
		private void RequestUpdateCountdownRpc(string text)
		{
			RequestUpdateCountdownRpc(text);
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void UpdateCountdownRpc(string text)
		{
			countdownText.text = text;
        }

		[Rpc(SendTo.ClientsAndHost)]
		private void CountdownFinishedRpc()
		{
            // Need to publish this event in the call back to the clients and host to ensure everyone recieves the event data.
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
	}
}
