using System;
using System.Collections;
using UnityEngine;

namespace HumbleGames
{
    public class Rocket : MonoBehaviour
    {
        // ---------------------------------- Inspector Visible ---------------------------------------

        [SerializeField]
        private bool enableParticles = true;

        [SerializeField]
        private ProbeMonitor probeMonitor;

        [SerializeField]
        private float powerMainEngine = 14f;

        [SerializeField]
        private float powerStopEngine = 7f;

        [SerializeField]
        private float powerSideEngines = 0.2f;

        [SerializeField]
        private Rigidbody rocketRb;

        [SerializeField]
        private ParticleSystem particlesEngineMain;

        [SerializeField]
        private ParticleSystem particlesEngineStop;

        [SerializeField]
        private ParticleSystem particlesEngineDownLeft;

        [SerializeField]
        private ParticleSystem particlesEngineDownRight;

        [SerializeField]
        private ParticleSystem particlesEngineUpLeft;

        [SerializeField]
        private ParticleSystem particlesEngineUpRight;

        [SerializeField]
        private bool useGravity = true;

        [Header("Markers")]
        [SerializeField]
        private GameObject markerRocket;

        [SerializeField]
        private GameObject markerEngineDownLeft;

        [SerializeField]
        private GameObject markerEngineDownRight;

        [SerializeField]
        private GameObject markerEngineUpLeft;

        [SerializeField]
        private GameObject markerEngineUpRight;

        [SerializeField]
        private GameObject markerEngineMain;

        [SerializeField]
        private GameObject markerSpherePrefab;

        [Header("Force Application Positions")]
        [SerializeField]
        private Vector3 forcePosEngineLeft = new Vector3(-0.5f, -1f, 0);

        [SerializeField]
        private Vector3 forcePosEngineRight = new Vector3(0.5f, -1f, 0);

        [SerializeField]
        private Vector3 forcePosEngineMain = new Vector3(0, -2.2f, 0);

        [Header("Force Directions")]
        [SerializeField]
        private float forceDirectionAngleLeft = 120f;

        [SerializeField]
        private float forceDirectionAngleRight = -120f;

        // ----------------------------------- Private ------------------------------------------------

        private Vector3 forceDirectionEngineDownLeft = Vector3.down;
        private Vector3 forceDirectionEngineDownRight = Vector3.down;
        private Vector3 forceDirectionEngineMain = Vector3.down;
        private Vector3 forceDirectionEngineUpLeft = Vector3.down;
        private Vector3 forceDirectionEngineUpRight = Vector3.down;

        private bool resetInProgress = false;
        private Vector3 centerOfMassPos;
        private GameObject markerCenterOfMass;

        private static float StartPosX;
        private static float StartPosY;

        // --------------------------------------------------------------------------------------------
        //                               Unity Lifecycle Methods
        // --------------------------------------------------------------------------------------------

        public void Awake()
        {
            StartPosX = transform.position.x;
            StartPosY = transform.position.y;
        }

        // --------------------------------------------------------------------------------------------
        //                                    Public API
        // --------------------------------------------------------------------------------------------

        public void Initialize(float startPosX, float startPosY)
        {
            rocketRb.useGravity = useGravity;

            StartPosX = startPosX;
            StartPosY = startPosY;

            transform.position = new Vector3(StartPosX, StartPosY);

            // Draw marker at Rocket's center
            markerRocket.transform.parent = rocketRb.transform; // make rocketMarker child of rocket

            UpdateEngineForceDirections();

            // align particle systems with force directions
            particlesEngineDownLeft.transform.position = markerEngineDownLeft.transform.position;
            particlesEngineDownLeft.transform.rotation = Quaternion.LookRotation(forceDirectionEngineDownLeft);
            particlesEngineDownRight.transform.position = markerEngineDownRight.transform.position;
            particlesEngineDownRight.transform.rotation = Quaternion.LookRotation(forceDirectionEngineDownRight);

            particlesEngineUpLeft.transform.position = markerEngineUpLeft.transform.position;
            particlesEngineUpLeft.transform.rotation = Quaternion.LookRotation(forceDirectionEngineUpLeft);
            particlesEngineUpRight.transform.position = markerEngineUpRight.transform.position;
            particlesEngineUpRight.transform.rotation = Quaternion.LookRotation(forceDirectionEngineUpRight);

            InstantiateCetreOfMassMarker();
        }

        public void Reset(Vector3 rocketPos)
        {
            resetInProgress = true;

            rocketRb.constraints = RigidbodyConstraints.FreezeAll;

            StartPosX = rocketPos.x;
            StartPosY = rocketPos.y;
            Reset();
        }

        private void Reset()
        {
            // reset rocket's movement, rotation, position
            rocketRb.velocity = Vector3.zero;
            rocketRb.angularVelocity = Vector3.zero;
            rocketRb.transform.rotation = Quaternion.identity;

            rocketRb.angularDrag = 0f;
            transform.localPosition = new Vector3(StartPosX, StartPosY);
            probeMonitor.Reset();
            markerCenterOfMass.transform.position = GetCenterOfMass();

            resetInProgress = false;
        }

        public void Update()
        {
            /*
             * Workaround: Freezing all constraints in Reset() and unfreezing them here in order to
             * avoid rocket rotation issue when new session is started:
             * 
             * This is required to reset rocket rotation when switching to a new session - when
             * rocket moves into left or right vertical wall (presumably at a high speed) and 
             * forward button (main engine) is pressed - the session is over with a failure (that
             * is expected), but the new session is started with rocket rotating around z-axis 
             * rapidly - it looks like in this case Agent.Reset() logic is not enough... Why?
             * 
             */
            if (rocketRb.constraints == RigidbodyConstraints.FreezeAll)
            {
                rocketRb.constraints = RigidbodyConstraints.None;
                rocketRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            }

            markerCenterOfMass.transform.position = GetCenterOfMass();
        }

        public void UpdateEngineForceDirections()
        {
            forceDirectionEngineMain = markerRocket.transform.position - markerEngineMain.transform.position;
            forceDirectionEngineDownLeft = Quaternion.AngleAxis(forceDirectionAngleLeft, Vector3.forward) * forceDirectionEngineMain;
            forceDirectionEngineDownRight = Quaternion.AngleAxis(forceDirectionAngleRight, Vector3.forward) * forceDirectionEngineMain;

            forceDirectionEngineUpLeft = Quaternion.AngleAxis(forceDirectionAngleLeft, Vector3.forward) * forceDirectionEngineMain;
            forceDirectionEngineUpRight = Quaternion.AngleAxis(forceDirectionAngleRight, Vector3.forward) * forceDirectionEngineMain;

            Debug.DrawRay(markerEngineDownLeft.transform.position, forceDirectionEngineDownLeft, Color.white);
            Debug.DrawRay(markerEngineDownRight.transform.position, forceDirectionEngineDownRight, Color.white);

            Debug.DrawRay(markerEngineUpLeft.transform.position, forceDirectionEngineUpLeft, Color.yellow);
            Debug.DrawRay(markerEngineUpRight.transform.position, forceDirectionEngineUpRight, Color.yellow);

            Debug.DrawRay(markerEngineMain.transform.position, forceDirectionEngineMain, Color.white);
        }

        public float GetRocketRotationZ()
        {
            return transform.rotation.eulerAngles.z / 180.0f - 1.0f;
        }

        public float GetAngularVelocityZ()
        {
            return rocketRb.angularVelocity.z;
        }

        public float GetLocalPosX()
        {
            return transform.localPosition.x;
        }

        public float GetLocalPosY()
        {
            return transform.localPosition.y;
        }

        public Vector3 GetLocalPosition()
        {
            return transform.localPosition;
        }

        public Vector3 GetVelocity()
        {
            return rocketRb.velocity;
        }

        public float GetVelocityMagnitude()
        {
            return Math.Abs(rocketRb.velocity.magnitude);
        }

        public float GetOrientation()
        {
            Vector3 engineToRocketVector = rocketRb.transform.position - markerEngineMain.transform.position;

            return Vector3.Dot(Vector3.right, engineToRocketVector);
        }

        public float GetTargetingAngle(GameObject target)
        {
            Vector3 targetToRocketVector = target.transform.position - markerRocket.transform.position;
            Vector3 rocketToEngineMainVector = markerRocket.transform.position - markerEngineMain.transform.position;

            return Vector3.Angle(targetToRocketVector, rocketToEngineMainVector);
        }

        // --------------------------------------------------------------------------------------------
        //                 Simple Movements (not used in simulation that uses forces)
        // --------------------------------------------------------------------------------------------

        private const float SIMPLE_MOVEMENT_DELTA = 0.15f;

        public void MoveLeft()
        {
            transform.localPosition = new Vector3(transform.localPosition.x + SIMPLE_MOVEMENT_DELTA,
                                            transform.localPosition.y,
                                            transform.localPosition.z);
            if (enableParticles)
            {
                particlesEngineDownLeft.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineDownLeft, 0.3f));
            }
        }

        public void MoveRight()
        {
            transform.localPosition = new Vector3(transform.localPosition.x - SIMPLE_MOVEMENT_DELTA,
                            transform.localPosition.y,
                            transform.localPosition.z);

            if (enableParticles)
            {
                particlesEngineDownRight.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineDownRight, 0.3f));
            }
        }

        public void MoveUp()
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                    transform.localPosition.y + SIMPLE_MOVEMENT_DELTA,
                    transform.localPosition.z);

            if (enableParticles)
            {
                particlesEngineMain.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineMain, 0.3f));
            }
        }

        public void MoveDown()
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                    transform.localPosition.y - SIMPLE_MOVEMENT_DELTA,
                    transform.localPosition.z);

            if (enableParticles)
            {
                particlesEngineStop.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineStop, 0.3f));
            }
        }

        // --------------------------------------------------------------------------------------------
        //                                Engine Simulation Movements
        // --------------------------------------------------------------------------------------------

        public bool IsOnPlanet()
        {
            return probeMonitor.IsOnPlanet();
        }

        public bool AccidentOccured()
        {
            return probeMonitor.AccidentOccured();
        }

        public void EngineMainEmitBurst()
        {
            if (resetInProgress)
            {
                return;
            }

            rocketRb.AddForceAtPosition(
                forceDirectionEngineMain.normalized * powerMainEngine,
                markerEngineMain.transform.position,
                ForceMode.Acceleration);

            if (enableParticles)
            {
                particlesEngineMain.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineMain, 0.3f));
            }
        }

        public void EngineStopEmitBurst()
        {
            rocketRb.AddForceAtPosition(
                (-1) * forceDirectionEngineMain.normalized * powerStopEngine,
                markerCenterOfMass.transform.position,
                ForceMode.Acceleration);

            if (enableParticles)
            {
                particlesEngineStop.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineStop, 0.3f));
            }
        }

        public void EngineDownLeftEmitBurst()
        {
            rocketRb.AddForceAtPosition(
                forceDirectionEngineDownLeft.normalized * (-1) * powerSideEngines,
                markerEngineDownLeft.transform.position,
                ForceMode.Acceleration);

            if (enableParticles)
            {
                particlesEngineDownLeft.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineDownLeft, 0.3f));
            }
        }

        public void EngineDownRightEmitBurst()
        {
            rocketRb.AddForceAtPosition(
                forceDirectionEngineDownRight.normalized * (-1) * powerSideEngines,
                markerEngineDownRight.transform.position,
                ForceMode.Acceleration);

            if (enableParticles)
            {
                particlesEngineDownRight.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineDownRight, 0.3f));
            }
        }


        public void EngineUpLeftEmitBurst()
        {
            rocketRb.AddForceAtPosition(
                forceDirectionEngineUpLeft.normalized * (-1) * powerSideEngines,
                markerEngineUpLeft.transform.position,
                ForceMode.Acceleration);

            if (enableParticles)
            {
                particlesEngineUpLeft.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineUpLeft, 0.3f));
            }
        }

        public void EngineUpRightEmitBurst()
        {
            rocketRb.AddForceAtPosition(
                forceDirectionEngineUpRight.normalized * (-1) * powerSideEngines,
                markerEngineUpRight.transform.position,
                ForceMode.Acceleration);

            if (enableParticles)
            {
                particlesEngineUpRight.Play();
                StartCoroutine(StopParticlesAfterDelay(particlesEngineUpRight, 0.3f));
            }
        }

        // --------------------------------------------------------------------------------------------
        //                                  Private Helpers
        // --------------------------------------------------------------------------------------------

        private IEnumerator StopParticlesAfterDelay(ParticleSystem particlesToStop, float delay)
        {
            yield return new WaitForSeconds(delay);
            particlesToStop.Stop();
        }

        private void InstantiateCetreOfMassMarker()
        {
            centerOfMassPos = GetCenterOfMass();

            markerCenterOfMass = Instantiate(
                markerSpherePrefab, centerOfMassPos, Quaternion.identity, rocketRb.transform);
            markerCenterOfMass.name = "MarkerSphere_CenterOfMass";
        }

        // see this for more details:
        // https://forum.unity.com/threads/get-centre-point-of-multiple-child-objects.131921/
        private Vector3 GetCenterOfMass()
        {
            Collider[] allChildren = transform.gameObject.GetComponentsInChildren<Collider>();

            int numOfObjectsContributingToCenterOfMass = 0;

            foreach (Collider child in allChildren)
            {
                if (!child.gameObject.CompareTag("EXCLUDE_FROM_CENTER_OF_MASS"))
                {
                    centerOfMassPos += child.transform.position * 1; // * weight
                    numOfObjectsContributingToCenterOfMass++;
                }
            }

            centerOfMassPos /= (numOfObjectsContributingToCenterOfMass + 1); // +1 for the root node

            return centerOfMassPos;
        }
    }
}