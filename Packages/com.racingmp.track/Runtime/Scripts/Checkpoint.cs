using System.Collections.Generic;
using packageBase.core;
using Unity.Netcode;
using UnityEngine;

namespace racingMP.track
{
    public class Checkpoint : MonoBehaviour
    {
		[SerializeField] private bool isFinish;

		private List<ulong> passedCars = new();

		/// <summary>
		/// If the car hitting this checkpoint has not passed it yet, mark it and signal a checkpoint event
		/// </summary>
		/// <param name="other">The car colliding with the checkpoint. This will ALWAYS be a collider attached to a Car instance.</param>
		private void OnTriggerEnter(Collider other)
		{
			ulong clientId = other.transform.root.GetComponent<NetworkObject>().OwnerClientId;

			if (isFinish)
			{
				EventManager.Instance.PublishEvent(new EventCheckpointHit() { ClientId = clientId, IsFinish = isFinish });
			}
			else if (!passedCars.Contains(clientId))
			{
				passedCars.Add(clientId);
				EventManager.Instance.PublishEvent(new EventCheckpointHit() { ClientId = clientId, IsFinish = isFinish });
			}
		}
	}
}
