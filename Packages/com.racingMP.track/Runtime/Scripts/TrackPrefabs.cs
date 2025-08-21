using UnityEngine;

namespace racingMP.track 
{
	[CreateAssetMenu(fileName = "TrackPrefabs", menuName = "Scriptable Objects/TrackPrefabs")]
	public class TrackPrefabs : ScriptableObject
	{
		public GameObject startPrefab;
		public GameObject finishPrefab;
		public GameObject checkPointPrefab;

		public GameObject[] trackComponentPrefabs;
		public GameObject[] completeTrackPrefabs;
	}
}
