using packageBase.core;
using packageBase.input;
using Unity.Netcode;
using UnityEngine;

namespace racingMP.player
{
    /// <summary>
    /// Class representing a car.
    /// Inherits from Unity Functions in <see cref="packageBase.core"/>.
    /// </summary>
    public class Car : NetworkBehaviour, ISystem, ISubscriber<EventRaceStarted>
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

        private bool canMove = false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = _centerOfMass;
        }

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

            // Let the track manager know that this car has spawned in
            EventManager.Instance.PublishEvent(new EventCarSpawn() { NetObjId = NetworkObjectId });
            EventManager.Instance.SubscribeEvent(typeof(EventRaceStarted), this);

            // If not the owner of this car, remove the camera to ensure only yours exists on your client side.
            if (!IsOwner)
            {
                Destroy(transform.Find("Main Camera").gameObject);
            }
		}

		private void Start()
        {
            _playerInput = ReferenceManager.Instance.GetReference<PlayerInput>();
        }

        private void Update()
        {
            if (!IsOwner || !canMove) return;

            // Handling functionality for all wheels.
            foreach (Wheel wheel in _wheels)
            {
                wheel.GenerateTorque(_drive, _torque * _playerInput.MoveInput.y);
                wheel.Steer(_playerInput.MoveInput.x * _turnSensitivity * _maxSteerAngle);
            }
        }

		public void OnEventHandler(in EventRaceStarted e)
		{
            canMove = true;
		}
	}
}
