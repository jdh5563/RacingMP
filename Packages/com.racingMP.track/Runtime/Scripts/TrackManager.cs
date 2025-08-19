using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using packageBase.core;
using racingMP.player;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace racingMP.track
{
	public class TrackManager : NetworkBehaviour, ISystem, ISubscriber<EventCarSpawn>
	{
		[SerializeField] private TrackPrefabs trackPrefabsSO;
		[SerializeField] private int gridWidth;
		[SerializeField] private int gridHeight;
		[SerializeField] private int gridDepth;

		public Dictionary<string, GameObject> trackComponentsByName = new();
		public Dictionary<string, GameObject> completeTracksByName = new();

		private Track[,,] worldGrid;
		private const int CELL_SIZE = 10;

		private Track startBlock = null;

		private Vector3Int currentPos = Vector3Int.zero;

		private int spawnIndex = 0;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			if (IsServer)
			{
				EventManager.Instance.SubscribeEvent(typeof(EventCarSpawn), this);

				worldGrid = new Track[gridWidth, gridHeight, gridDepth];

				foreach (GameObject track in trackPrefabsSO.trackComponentPrefabs)
				{
					trackComponentsByName.Add(track.name, track);
				}

				foreach (GameObject track in trackPrefabsSO.completeTrackPrefabs)
				{
					completeTracksByName.Add(track.name, track);
				}

				GenerateTrack();

				foreach (NetworkObject playerObj in NetworkManager.Singleton.SpawnManager.PlayerObjects)
				{
					MoveCarToStartRpc(playerObj.NetworkObjectId);
				}
			}
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		// Update is called once per frame
		private void Update()
		{

		}

		/// <summary>
		/// ITERATION #1: Generates a simple circuit
		/// </summary>
		private void GenerateTrack()
		{
			// The code on the right is here to show the power I hold but choose not to use -> SpawnTrackComponent(trackComponentsByName["Turn Short"], SpawnTrackComponent(trackComponentsByName["Straight Long"], SpawnTrackComponent(trackPrefabsSO.checkPointPrefab, SpawnTrackComponent(trackComponentsByName["Turn Short"], SpawnTrackComponent(trackComponentsByName["Straight Long"], SpawnTrackComponent(trackPrefabsSO.checkPointPrefab, SpawnTrackComponent(trackComponentsByName["Turn Short"], SpawnTrackComponent(trackComponentsByName["Straight Long"], SpawnTrackComponent(trackPrefabsSO.checkPointPrefab, SpawnTrackComponent(trackComponentsByName["Turn Short"], SpawnTrackComponent(trackComponentsByName["Straight Long"], SpawnTrackComponent(trackPrefabsSO.startPrefab, new Vector3Int(gridWidth / 2, gridHeight / 2, gridDepth / 2), Quaternion.identity).endPoint.position, Quaternion.identity).endPoint.position, Quaternion.Euler(0, 90, 0)).endPoint.position, Quaternion.Euler(0, 90, 0)).endPoint.position, Quaternion.Euler(0, 90, 0)).endPoint.position, Quaternion.Euler(0, 180, 0)).endPoint.position, Quaternion.Euler(0, 180, 0)).endPoint.position, Quaternion.Euler(0, 180, 0)).endPoint.position, Quaternion.Euler(0, -90, 0)).endPoint.position, Quaternion.Euler(0, -90, 0)).endPoint.position, Quaternion.Euler(0, -90, 0)).endPoint.position, Quaternion.identity);
			currentPos = new Vector3Int(gridWidth / 2, gridHeight / 2, gridDepth / 2);
			startBlock = SpawnTrackComponent(trackPrefabsSO.startPrefab, currentPos, Quaternion.identity);

			Track track = SpawnTrackComponent(trackComponentsByName["Straight Long"], startBlock.endPoint.position, Quaternion.identity);
			track = SpawnTrackComponent(trackComponentsByName["Turn Short"], track.endPoint.position, Quaternion.Euler(0, 90, 0));

			track = SpawnTrackComponent(trackPrefabsSO.checkPointPrefab, track.endPoint.position, Quaternion.Euler(0, 90, 0));
			track = SpawnTrackComponent(trackComponentsByName["Straight Long"], track.endPoint.position, Quaternion.Euler(0, 90, 0));
			track = SpawnTrackComponent(trackComponentsByName["Turn Short"], track.endPoint.position, Quaternion.Euler(0, 180, 0));

			track = SpawnTrackComponent(trackPrefabsSO.checkPointPrefab, track.endPoint.position, Quaternion.Euler(0, 180, 0));
			track = SpawnTrackComponent(trackComponentsByName["Straight Long"], track.endPoint.position, Quaternion.Euler(0, 180, 0));
			track = SpawnTrackComponent(trackComponentsByName["Turn Short"], track.endPoint.position, Quaternion.Euler(0, -90, 0));

			track = SpawnTrackComponent(trackPrefabsSO.checkPointPrefab, track.endPoint.position, Quaternion.Euler(0, -90, 0));
			track = SpawnTrackComponent(trackComponentsByName["Straight Long"], track.endPoint.position, Quaternion.Euler(0, -90, 0));
			SpawnTrackComponent(trackComponentsByName["Turn Short"], track.endPoint.position, Quaternion.identity);
		}

		/// <summary>
		/// Spawn the given track component at the given grid position
		/// </summary>
		private Track SpawnTrackComponent(GameObject trackComponent, Vector3 worldPos, Quaternion rot)
		{
			Vector3Int gridPos = new Vector3Int((int)worldPos.x, (int)worldPos.y, (int)worldPos.z) / CELL_SIZE;
			GameObject trackObj = Instantiate(trackComponent);
			Track track = trackObj.GetComponent<Track>();

			trackObj.transform.SetPositionAndRotation(worldPos, rot);

			worldGrid[gridPos.x, gridPos.y, gridPos.z] = track;
			foreach (Vector3Int index in track.additionalIndexes)
			{
				Vector3 indexRot = rot * index;
				// Rounding instead of int-casting because of floating point error in the vector rotation
				Vector3Int indexRotInt = new Vector3Int(Mathf.RoundToInt(indexRot.x), Mathf.RoundToInt(indexRot.y), Mathf.RoundToInt(indexRot.z));
				worldGrid[gridPos.x + indexRotInt.x, gridPos.y + indexRotInt.y, gridPos.z + indexRotInt.z] = track;
			}

			trackObj.GetComponent<NetworkObject>().Spawn();

			return track;
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void MoveCarToStartRpc(ulong networkObjectId)
		{
			NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].transform.position = startBlock.transform.Find("SpawnPoints").GetChild(spawnIndex++).position;
		}

		[Rpc(SendTo.Server)]
		private void RequestMoveCarToStartRpc(ulong networkObjectId)
		{
			MoveCarToStartRpc(networkObjectId);
		}

		public void OnEventHandler(in EventCarSpawn e)
		{
			RequestMoveCarToStartRpc(e.NetObjId);
		}
	}

}