using packageBase.core;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace racingMP.track
{
	public class TrackManager : NetworkBehaviour, ISystem, ISubscriber<EventCarSpawn>, ISubscriber<EventResetLevel>
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
		private Track finishBlock = null;

		private Vector3Int currentPos = Vector3Int.zero;

		private int spawnIndex = 0;

		private int totalCheckpoints = 0;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			if (IsServer)
			{
				EventManager.Instance.SubscribeEvent(typeof(EventCarSpawn), this);

				InitLevel();
			}
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		private void Start()
		{
			DontDestroyOnLoad(gameObject);

			EventManager.Instance.SubscribeEvent(typeof(EventResetLevel), this);
		}

		// Update is called once per frame
		private void Update()
		{

		}

		private void InitLevel()
		{
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

			EventManager.Instance.PublishEvent(new EventTrackGenerated() { TotalCheckpoints = totalCheckpoints });

			foreach (NetworkObject playerObj in NetworkManager.Singleton.SpawnManager.PlayerObjects)
			{
				MoveCarToStartRpc(playerObj.NetworkObjectId, startBlock.transform.Find("SpawnPoints").GetChild(spawnIndex++).position, startBlock.transform.forward);
			}
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
			totalCheckpoints++;
			track = SpawnTrackComponent(trackComponentsByName["Straight Long"], track.endPoint.position, Quaternion.Euler(0, 90, 0));
			track = SpawnTrackComponent(trackComponentsByName["Turn Short"], track.endPoint.position, Quaternion.Euler(0, 180, 0));

			track = SpawnTrackComponent(trackPrefabsSO.checkPointPrefab, track.endPoint.position, Quaternion.Euler(0, 180, 0));
			totalCheckpoints++;
			track = SpawnTrackComponent(trackComponentsByName["Straight Long"], track.endPoint.position, Quaternion.Euler(0, 180, 0));
			track = SpawnTrackComponent(trackComponentsByName["Turn Short"], track.endPoint.position, Quaternion.Euler(0, -90, 0));

			track = SpawnTrackComponent(trackPrefabsSO.checkPointPrefab, track.endPoint.position, Quaternion.Euler(0, -90, 0));
			totalCheckpoints++;
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
		private void MoveCarToStartRpc(ulong networkObjectId, Vector3 spawnPosition, Vector3 spawnForward)
		{
			NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].transform.SetPositionAndRotation(spawnPosition, Quaternion.LookRotation(spawnForward));
		}

		[Rpc(SendTo.Server)]
		private void RequestMoveCarToStartRpc(ulong networkObjectId)
		{
			MoveCarToStartRpc(networkObjectId, startBlock.transform.Find("SpawnPoints").GetChild(spawnIndex++).position, startBlock.transform.forward);
		}

		public void OnEventHandler(in EventCarSpawn e)
		{
			RequestMoveCarToStartRpc(e.NetObjId);
		}

		public void OnEventHandler(in EventResetLevel e)
		{
			if (!IsServer) return;

			spawnIndex = 0;

			foreach(NetworkObject player in NetworkManager.Singleton.SpawnManager.PlayerObjects)
			{
				MoveCarToStartRpc(player.NetworkObjectId, startBlock.transform.Find("SpawnPoints").GetChild(spawnIndex++).position, startBlock.transform.forward);
			}
		}
	}
}