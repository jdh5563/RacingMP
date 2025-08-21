using System.Collections.Generic;
using packageBase.core;
using Unity.Netcode;
using UnityEngine;

namespace racingMP.track
{
    public class Checkpoint : MonoBehaviour
    {
		private List<ulong> pastCars = new();

		/// <summary>
		/// If the car hitting this checkpoint has not passed it yet, mark it and signal a checkpoint event
		/// </summary>
		/// <param name="other">The car colliding with the checkpoint. This will ALWAYS be a collider attached to a Car instance.</param>
		private void OnTriggerEnter(Collider other)
		{
			ulong netObjId = other.transform.root.GetComponent<NetworkObject>().NetworkObjectId;

			if (!pastCars.Contains(netObjId))
            {
                pastCars.Add(netObjId);
				EventManager.Instance.PublishEvent(new EventCheckpointHit() { NetObjId = netObjId });
			}
		}
	}
}
