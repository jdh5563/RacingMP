using packageBase.core;
using Unity.Netcode;
using UnityEngine;

namespace racingMP.player
{
    /// <summary>
    /// Class representing a car.
    /// Inherits from InitableBase in <see cref="packageBase.core"/>.
    /// </summary>
    public class Car : NetworkBehaviour
    {
        [SerializeField]
        private Drive _drive;

        [SerializeField]
        private Vector3 _centerOfMass;

        [SerializeField]
        private float _torque;

        [SerializeField]
        private float _turnSensitivity;

        [SerializeField]
        private float _maxSteerAngle;

        [SerializeField]
        private Wheel[] _wheels;

        private Rigidbody _rb;

        private PlayerInput _playerInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = _centerOfMass;
        }

        private void Start()
        {
            _playerInput = ReferenceManager.Instance.GetReference<PlayerInput>();
        }

        /*public override void DoInit()
        {
            base.DoInit();

            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = _centerOfMass;

            ReferenceManager.Instance.AddReference<Car>(this);
        }*/

        /*public override void DoPostInit()
        {
            base.DoPostInit();

            _playerInput = ReferenceManager.Instance.GetReference<PlayerInput>();
        }*/

        private void FixedUpdate()
        {
            // Handling functionality for all wheels.
            foreach (Wheel wheel in _wheels)
            {
                wheel.GenerateTorque(_drive, _torque * _playerInput.MoveInput.y);
                wheel.Steer(_playerInput.MoveInput.x * _turnSensitivity * _maxSteerAngle);
            }
        }
    }
}
