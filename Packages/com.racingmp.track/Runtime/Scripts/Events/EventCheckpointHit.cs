namespace racingMP.track
{
	public struct EventCheckpointHit
	{
		// The ID of the network object associated with the car that hit this checkpoint
		public ulong NetObjId {  get; set; }

		// Whether this checkpoint is a finish or not
		public bool IsFinish {  get; set; }
	}
}
