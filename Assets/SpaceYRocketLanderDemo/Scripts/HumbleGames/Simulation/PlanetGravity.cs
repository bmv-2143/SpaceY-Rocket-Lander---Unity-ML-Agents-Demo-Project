using HumbleGames.Utils;
using UnityEngine;

namespace HumbleGames.Simulation
{
    public class PlanetGravity : MonoBehaviour
    {

        public Rigidbody gravityAffectedObject;
        public float gravityRange;
        public float gravityStrength = 25;

        private void Start()
        {
            DrawCircle drawCircleScript = GetComponent<DrawCircle>();
            drawCircleScript.radiusXY = gravityRange;
        }

        private void FixedUpdate()
        {
            ApplyPlanetGravity();
        }

        private void ApplyPlanetGravity()
        {
            if (IsInGravityRange(gravityAffectedObject))
            {
                Vector3 movementDirection = transform.position - gravityAffectedObject.transform.position;

                Vector3 movementDirectionNormalized =
                         new Vector3(movementDirection.x, movementDirection.y, 0).normalized;

                Vector3 planetGravityForce = movementDirectionNormalized * gravityStrength;

                gravityAffectedObject.AddForce(planetGravityForce, ForceMode.Acceleration);
            }
        }

        private bool IsInGravityRange(Rigidbody objectForGravity)
        {
            return Vector3.Distance(
                    transform.position,
                    objectForGravity.transform.position) <= gravityRange;
        }
    }
}