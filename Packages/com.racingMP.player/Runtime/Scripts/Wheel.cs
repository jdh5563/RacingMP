using System;
using UnityEngine;

namespace racingMP.player
{
    /// <summary>
    /// Struct used to represent each of the wheels on a <see cref="Car"/>.
    /// </summary>
    [Serializable]
    public struct Wheel
    {
        [SerializeField]
        private GameObject model;

        [SerializeField]
        private WheelCollider collider;

        [SerializeField]
        private Axle axle;

        /// <summary>
        /// Function called in <see cref="Car"/> to handle generating torque moving the car forward and back.
        /// </summary>
        /// <param name="driveType">The drive type set in <see cref="Car"/></param>
        /// <param name="torqueFromInput">The torque calculated from the player move input and the car's torque value.</param>
        public void GenerateTorque(Drive driveType, float torqueFromInput)
        {
            // If drive type is all, apply torque to all wheels.
            if (driveType == Drive.All)
            {
                collider.motorTorque = torqueFromInput;
            }
            // If drive type is rear, only apply torque to rear wheels.
            else if (driveType == Drive.Rear)
            {
                if (axle == Axle.Rear)
                {
                    collider.motorTorque = torqueFromInput;
                }
            }
            else if (driveType == Drive.Front)
            {
                if (axle == Axle.Front)
                {
                    collider.motorTorque = torqueFromInput;
                }
            }
        }

        /// <summary>
        /// Function called in <see cref="Car"/> to handle steering the car and animating the wheels.
        /// </summary>
        /// <param name="steerAngle">The steering angle calculated from the player move input, turn sensitivity, and max steer angle in <see cref="Car"/></param>
        public void Steer(float steerAngle)
        {
            if (axle == Axle.Front)
            {
                collider.steerAngle = steerAngle;
            }

            collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            model.transform.SetPositionAndRotation(position, rotation);
        }
    }
}
