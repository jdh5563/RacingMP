using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private TrackPrefabs trackPrefabs;

    private GameObject startBlock = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTrack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateTrack()
    {
        //Instantiate(trackPrefabs.completeTracksByName["Simple Circuit"]);
        Instantiate(trackPrefabs.completeTrackPrefabs[0]);
	}
}
