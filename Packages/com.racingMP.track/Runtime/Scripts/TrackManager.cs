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
			Instantiate(completeTracksByName["Simple Circuit"]);
			//startBlock = SpawnTrackComponent(trackPrefabsSO.startPrefab, new Vector3Int(gridWidth / 2, gridHeight / 2, gridDepth / 2), Quaternion.identity);

			//SpawnTrackComponent(trackComponentsByName["Straight Long"], currentPos, Quaternion.identity);
			//SpawnTrackComponent(trackComponentsByName["Turn Short"], currentPos, Quaternion.identity);

			//SpawnTrackComponent(trackComponentsByName["Straight Long"], currentPos, Quaternion.Euler(0, 90, 0));
			//SpawnTrackComponent(trackComponentsByName["Turn Short"], currentPos, Quaternion.Euler(0, 90, 0));

			//SpawnTrackComponent(trackComponentsByName["Straight Long"], currentPos, Quaternion.identity);
			//SpawnTrackComponent(trackComponentsByName["Turn Short"], currentPos, Quaternion.Euler(0, 180, 0));

			//SpawnTrackComponent(trackComponentsByName["Straight Long"], currentPos, Quaternion.Euler(0, 90, 0));
			//SpawnTrackComponent(trackComponentsByName["Turn Short"], currentPos, Quaternion.Euler(0, -90, 0));
		}

		/// <summary>
		/// Spawn the given track component at the given grid position
		/// </summary>
		private Track SpawnTrackComponent(GameObject trackComponent, Vector3Int pos, Quaternion rot)
		{
			GameObject trackObj = Instantiate(trackComponent, new Vector3(pos.x * CELL_SIZE, pos.y * CELL_SIZE, pos.z * CELL_SIZE), rot);
			Track track = trackObj.GetComponent<Track>();

			return track;
		}
	}

}