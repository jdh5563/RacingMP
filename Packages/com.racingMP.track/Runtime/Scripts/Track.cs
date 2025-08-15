using packageBase.core;
using UnityEngine;

namespace racingMP.track
{
	public class Track : InitableBase
	{
		public Vector3Int[] additionalIndexes;
		public Vector3Int direction;
		public Transform endPoint;
	}
}