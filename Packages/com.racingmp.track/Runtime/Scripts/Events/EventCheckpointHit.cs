namespace racingMP.track
{
	public struct EventCheckpointHit
	{
		// The ID of the client who owns the car that hit this checkpoint
		public ulong ClientId {  get; set; }

		// Whether this checkpoint is a finish or not
		public bool IsFinish {  get; set; }
	}
}
