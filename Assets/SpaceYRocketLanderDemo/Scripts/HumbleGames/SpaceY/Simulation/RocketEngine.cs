using UnityEngine;

namespace HumbleGames.SpaceY.Simulation
{
    public class RocketEngine : MonoBehaviour
    {
        /// <summary>
        /// Apply force along Y in the local space. Rotate the engine object accodingly to change the force 
        /// application direction.
        /// </summary>
        public void ApplyEngineForce(Rigidbody rocketRb, float forceValue)
        {
            // see Local Direction of Vector3
            // https://answers.unity.com/questions/634157/local-direction-of-vector3.html

            rocketRb.AddForceAtPosition(
            -transform.up * forceValue,
            transform.position,
            ForceMode.Acceleration);
        }
    }
}
