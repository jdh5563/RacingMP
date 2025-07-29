using packageBase.core;
using UnityEngine;

namespace racingMP.player
{
    public class Wheel : InitableBase
    {
        [SerializeField]
        private WheelCollider _wheelCollider;

        [SerializeField]
        private bool _wheelTurn;

        public override void DoInit()
        {
            base.DoInit();
        }

        private void Update()
        {
            if (_wheelTurn)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, _wheelCollider.steerAngle - transform.localEulerAngles.z, transform.localEulerAngles.z);
            }

            transform.Rotate(_wheelCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        }
    }
}
