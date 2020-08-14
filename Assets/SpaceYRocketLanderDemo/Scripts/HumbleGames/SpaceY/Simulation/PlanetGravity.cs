using HumbleGames.SpaceY.MLAgents;
using HumbleGames.SpaceY.Utils;
using UnityEngine;

namespace HumbleGames.SpaceY.Simulation
{
    public class PlanetGravity : MonoBehaviour
    {

        public Rigidbody gravityAffectedObject;
        public float gravityRange;
        public float gravityStrength = 25;

        // TODO: refactor, remove SimulationConfig dependency
        [SerializeField]
        private SimulationConfig simulationConfig;

        private void Start()
        {
            DrawCircle drawCircleScript = GetComponent<DrawCircle>();
            drawCircleScript.radiusXY = gravityRange;
            
            // TODO: refactor, it might be better to move this logic to object active state switcher
            drawCircleScript.enabled = simulationConfig.simulationMode == SimulationConfig.SimulationMode.Demo;
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