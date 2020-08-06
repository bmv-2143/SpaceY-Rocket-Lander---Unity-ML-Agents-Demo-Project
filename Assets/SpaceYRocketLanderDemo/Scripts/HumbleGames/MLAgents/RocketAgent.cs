using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace HumbleGames.MLAgents
{
    public class RocketAgent : Agent
    {
        private readonly string LOG_TAG = typeof(RocketAgent).Name;

        private const RocketAgent classHelper = null;
        
        [SerializeField]
        private TagHolder tagHolder;

        [SerializeField]
        private RewardConfig rewardConfig;

        [SerializeField]
        private GameObject leftLegLandingProbe;

        [SerializeField]
        private GameObject rightLegLandingProbe;

        // TODO: remove this dependency, use events instead
        [SerializeField]
        private SimulationArea simulationArea;

        [SerializeField]
        private GameObject basePlanet;
        
        [SerializeField]
        private GameObject targetPlanet;

        private SimulationState simulationState;
        private RocketNew rocketControl;
        private Rigidbody rocketRb;

        // ------------------------------------------------------------------------------------------------------------
        //                                Unity Lifecycle calls
        // ------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            rocketRb = GetComponent<Rigidbody>();
            rocketControl = GetComponent<RocketNew>();
            simulationState = GetComponent<SimulationState>();
        }

        private void FixedUpdate()
        {
            ProcessSimulationState();
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollisionEnterOrStay(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            HandleCollisionEnterOrStay(collision);
        }

        private void HandleCollisionEnterOrStay(Collision collision) 
        {
            //Debug.Log("Collision: " + collision.transform.name);

            // Leg probes landing (potential success)
            if (IsLeftLegProbeCollision(collision) && IsTargetPlanetCollision(collision))
            {
                simulationState.isLeftLegLanded = true;

            }

            if (IsRightLegProbeCollision(collision) && IsTargetPlanetCollision(collision))
            {
                simulationState.isRightLegLanded = true;
            }

            // Collision with a planet (failure)
            simulationState.isPlanetCollisionAccident =
                (IsTargetPlanetCollision(collision) || IsPlanetCollision(collision)) &&
                !(IsRightLegProbeCollision(collision) || IsLeftLegProbeCollision(collision));


            // Collision with DeathZones (failure)
            simulationState.isDeathZoneCollisionAccident = collision.transform.CompareTag(tagHolder.deathZone);
        }

        private bool IsLeftLegProbeCollision(Collision collision)
        {
            return collision.GetContact(0).thisCollider.CompareTag(tagHolder.legLeftLandingProbe);
        }

        private bool IsRightLegProbeCollision(Collision collision)
        {
            return collision.GetContact(0).thisCollider.CompareTag(tagHolder.legRightLandingProbe);
        }

        private bool IsTargetPlanetCollision(Collision collision)
        {
            return collision.GetContact(0).otherCollider.CompareTag(tagHolder.targetPlanet);
        }

        private bool IsPlanetCollision(Collision collision)
        {
            return collision.GetContact(0).otherCollider.CompareTag(tagHolder.planet);
        }

        // ------------------------------------------------------------------------------------------------------------
        //                                ML-Agents related calls
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initial setup, called when the agent is enabled
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Reset the agent and area
        /// </summary>
        public override void OnEpisodeBegin()
        {
            simulationState.Reset();
            Reset();
            simulationArea.Reset();
        }

        /// <summary>
        /// Collect all non-Raycast observations
        /// </summary>
        public override void CollectObservations(VectorSensor sensor)
        {
            // Distance from rocket to base planet          (+1 obsrvation)
            sensor.AddObservation(Vector3.Distance(transform.localPosition, basePlanet.transform.localPosition));

            // Distance from rocket to target planet        (+1 observation)
            sensor.AddObservation(Vector3.Distance(transform.localPosition, targetPlanet.transform.localPosition));

            // Direction to target planet                   (+3 observation)
            sensor.AddObservation((targetPlanet.transform.localPosition - transform.localPosition).normalized);

            // Rocket's orientation                         (+3 observations)
            sensor.AddObservation(transform.localEulerAngles);

            // Rocket's velocity                            (+3 observations)
            sensor.AddObservation(rocketRb.velocity);

            // Rocket's angular valocity                    (+3 observations)
            sensor.AddObservation(rocketRb.angularVelocity);

            // Is left leg landed                           (+1 observation)
            sensor.AddObservation(simulationState.isLeftLegLanded);

            // Is right let landed                          (+1 observation)
            sensor.AddObservation(simulationState.isRightLegLanded);

            // Is DeathZone collision accident              (+1 observation)
            sensor.AddObservation(simulationState.isDeathZoneCollisionAccident);

            // Is planet collision accident                 (+1 observation)
            sensor.AddObservation(simulationState.isPlanetCollisionAccident);


            // Total: 18 NON-RAYCAST observation (Raycast observations (if any) are auto-added -
            // see agent's RayPerciptionSensor 3D component (if it is added)
        }

        /// <summary>
        /// Perform actions based on a vector of numbers
        /// </summary>
        /// <param name="vectorAction">The list of actions to take</param>
        public override void OnActionReceived(float[] vectorAction)
        {
            ApplyActions(vectorAction);
            
            GiveStepwiseReward();
        }

        /// <summary>
        /// Read inputs from the keyboard and convert them to a list of actions.
        /// This is called only when the player wants to control the agent and has set
        /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
        /// </summary>
        /// <returns>A vectorAction array of floats that will be passed into <see cref="AgentAction(float[])"/></returns>
        public override void Heuristic(float[] actionsOut)
        {
            MapKeysToActions(actionsOut);
        }

        // ============================================================================================================
        // ============================================================================================================

        /// <summary>
        /// Resets <see cref="RocketAgent"/>
        /// </summary>
        private void Reset()
        {
            rocketControl.ResetRocket();
        }

        private void MapKeysToActions(float[] actionsOut)
        {
            // --------------------------------------------------------------------------------------------------------
            // Rocket Controls:
            // 
            //       S   
            //       ^
            //    Q | | E
            //      | |
            //      | |
            //    A | | D
            //     /   \
            //       W
            // 
            // --------------------------------------------------------------------------------------------------------

            // Move forward (main bottom\stern engine)
            if (Input.GetKey(KeyCode.W))
            {
                //Debug.LogFormat("{0}: {1}: Key: W", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 1f;
            }

            // Move backwards (auxiliary top\bow engine)
            else if (Input.GetKey(KeyCode.S))
            {
                //Debug.LogFormat("{0}: {1}: Key: S", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 2f;
            }

            // Top left auxiliary engine
            else if (Input.GetKey(KeyCode.Q))
            {
                //Debug.LogFormat("{0}: {1}: Key: Q", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 3f;
            }

            // Top right auxiliary engine
            else if (Input.GetKey(KeyCode.E))
            {
                //Debug.LogFormat("{0}: {1}: Key: E", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 4f;
            }

            // Bottom left auxiliary engine
            else if (Input.GetKey(KeyCode.A))
            {
                //Debug.LogFormat("{0}: {1}: Key: A", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 5f;
            }

            // Bottom right auxiliary engine
            else if (Input.GetKey(KeyCode.D))
            {
                //Debug.LogFormat("{0}: {1}: Key: D", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 6f;
            }

            // No keys detected - do nothing
            else
            {
                actionsOut[0] = 0;
            }
        }

        private void ApplyActions(float[] vectorAction)
        {
            // --------------------------------------------------------------------------------------------------------
            // Actions:
            //
            // vectorAction[0] == 0 - do nothing, engines are off
            // vectorAction[0] == 1 - BOTTOM MAIN engine is ON
            // vectorAction[0] == 2 - auxiliary TOP STOP engine is ON
            // vectorAction[0] == 3 - auxiliary TOP LEFT engine is ON
            // vectorAction[0] == 4 - auxiliary TOP RIGHT engine is ON
            // vectorAction[0] == 5 - auxiliary BOTTOM LEFT engine is ON
            // vectorAction[0] == 6 - auxiliary BOTTOM RIGHT engine is ON
            //
            // --------------------------------------------------------------------------------------------------------

            //Debug.LogFormat("{0}: {1}: Action: {2}", LOG_TAG, nameof(classHelper.ApplyActions), vectorAction[0]);

            switch (vectorAction[0]) // the first action branch
            {
                case 0:
                    // do nothing, don't turn on engines
                    break;

                case 1:
                    // MAIN BOTTOM engine is ON
                    rocketControl.ActivateMainBottomEngine();
                    break;

                case 2:
                    // auxiliary TOP STOP engine is ON
                    rocketControl.ActivateStopTopEngine();
                    break;

                case 3:
                    // auxiliary TOP LEFT engine is ON
                    rocketControl.ActivateAuxiliaryTopLeftEngine();
                    break;

                case 4:
                    // auxiliary TOP RIGHT engine is ON
                    rocketControl.ActivateAuxiliaryTopRightEngine();
                    break;

                case 5:
                    // auxiliary LEFT BOTTOM engine is ON
                    rocketControl.ActivateAuxiliaryBottomLeftEngine();
                    break;

                case 6:
                    // auxiliary RIGHT BOTTOM engine is ON
                    rocketControl.ActivateAuxiliaryBottomRightEngine();
                    break;

                default:
                    Debug.LogErrorFormat("{0}: {1}: Unexpected Action: {2}", LOG_TAG, nameof(classHelper.ApplyActions), vectorAction[0]);
                    break;
            }
        }

        private void ProcessSimulationState()
        {
            if (simulationState.isDeathZoneCollisionAccident)
            {
                //Debug.LogFormat("FAILURE: DeathZoneCollision", LOG_TAG);
                EventManager.RaiseSimulationEndEvent(SimulationEndStatus.FAILURE_DEATH_ZONE_COLLISION);
                GiveDeathZoneCollisionReward();
                EndEpisode();
                return;
            }

            if (simulationState.isPlanetCollisionAccident)
            {
                //Debug.LogFormat("FAILURE: PlanetCollision", LOG_TAG);
                EventManager.RaiseSimulationEndEvent(SimulationEndStatus.FAILURE_PLANET_COLLISION);
                GivePlanetCollisionReward();
                EndEpisode();
                return;
            }

            if (simulationState.IsLandingSucces())
            {
                //Debug.LogFormat("SUCCESS: Successfull landing", LOG_TAG);
                EventManager.RaiseSimulationEndEvent(SimulationEndStatus.SUCCESS);
                GiveLandingSuccessReward();
                EndEpisode();
                return;
            }
        }

        // ------------------------------------------------------------------------------------------------------------
        //                                          Rewards
        // ------------------------------------------------------------------------------------------------------------

        // TODO: move all rewarding object to a separate class (ScriptablObject)

        /// <summary>
        /// Applies a tiny negative reward every step to encourage action
        /// </summary>
        private void GiveStepwiseReward()
        {
            if (MaxStep > 0) 
            { 
                AddReward(rewardConfig.agentStepwiseRewardBase / MaxStep);
            }
        }

        private void GiveLandingSuccessReward()
        {
            AddReward(rewardConfig.landingSuccessReward);
        }

        private void GiveDeathZoneCollisionReward()
        {
            AddReward(rewardConfig.deathZoneCollisionReward);
        }

        private void GivePlanetCollisionReward()
        {
            AddReward(rewardConfig.planetCollisionReward);
        }
    }
}
