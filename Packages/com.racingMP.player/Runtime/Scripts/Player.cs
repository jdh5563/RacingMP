using packageBase.core;
using UnityEngine;

namespace racingMP.player
{
    public class Player : InitableBase
    {
        [SerializeField]
        private WheelCollider[] _wheelColliders;

        [SerializeField]
        private float _driveSpeed;

        [SerializeField]
        private float _steerSpeed;

        private Rigidbody _rb;

        private PlayerInput _playerInput;

        public override void DoInit()
        {
            base.DoInit();

            _rb = GetComponent<Rigidbody>();
            ReferenceManager.Instance.AddReference<Player>(this);
        }

        public override void DoPostInit()
        {
            base.DoPostInit();

            _playerInput = ReferenceManager.Instance.GetReference<PlayerInput>();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            //Vector3 moveDirection = new Vector3(_playerInput.MoveInput.x, 0.0f, _playerInput.MoveInput.y);

            float motor = _playerInput.MoveInput.y * _driveSpeed;

            foreach (WheelCollider wheelCollider in _wheelColliders)
            {
                wheelCollider.motorTorque = motor;
            }

            _wheelColliders[0].steerAngle = _steerSpeed * -_playerInput.MoveInput.x;
            _wheelColliders[1].steerAngle = _steerSpeed * -_playerInput.MoveInput.x;
        }
    }
}
