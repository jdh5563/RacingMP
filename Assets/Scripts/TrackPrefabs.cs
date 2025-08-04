using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrackPrefabs", menuName = "Scriptable Objects/TrackPrefabs")]
public class TrackPrefabs : ScriptableObject
{
	public GameObject startPrefab;
	public GameObject endPrefab;
	public GameObject checkPointPrefab;

	public GameObject[] trackComponentPrefabs;
	public GameObject[] completeTrackPrefabs;

	public Dictionary<string, GameObject> tracksComponentsByName = new();
	public Dictionary<string, GameObject> completeTracksByName = new();

	private void Awake()
	{
		foreach (GameObject track in trackComponentPrefabs)
		{
			tracksComponentsByName.Add(track.name, track);
		}

		foreach (GameObject track in completeTrackPrefabs)
		{
			completeTracksByName.Add(track.name, track);
		}
	}
}
