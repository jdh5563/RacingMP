using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace racingMP.track
{
	public class TrackManager : MonoBehaviour
	{
		[SerializeField] private TrackPrefabs trackPrefabsSO;
		[SerializeField] private int gridWidth;
		[SerializeField] private int gridHeight;
		[SerializeField] private int gridDepth;

		public Dictionary<string, GameObject> trackComponentsByName = new();
		public Dictionary<string, GameObject> completeTracksByName = new();

		private Track[,,] worldGrid;
		private Track startBlock = null;
		private const int CELL_SIZE = 10;

		private Vector3Int currentPos = Vector3Int.zero;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		private void Start()
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
			//Instantiate(completeTracksByName["Simple Circuit"]);
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

			return track;
		}
	}

}