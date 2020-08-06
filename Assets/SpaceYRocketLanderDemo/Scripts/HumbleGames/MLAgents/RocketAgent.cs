using System;
using System.Collections;
using System.Collections.Generic;
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
        private SimulationState simulationState;

        [SerializeField]
        private TagHolder tagHolder;

        [SerializeField]
        private RewardConfig rewardConfig;

        [SerializeField]
        private RocketNew rocket;

        [SerializeField]
        private GameObject leftLegLandingProbe;

        [SerializeField]
        private GameObject rightLegLandingProbe;

        // TODO: remove this dependency, use events instead
        [SerializeField]
        private SimulationArea simulationArea;

        // ------------------------------------------------------------------------------------------------------------
        //                                Unity Lifecycle calls
        // ------------------------------------------------------------------------------------------------------------

        // Update is called once per frame
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
            //throw new NotImplementedException();
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
            rocket.ResetRocket();
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

            switch (vectorAction[0])
            {
                case 0:
                    // do nothing, don't turn on engines
                    break;

                case 1:
                    // MAIN BOTTOM engine is ON
                    rocket.ActivateMainBottomEngine();
                    break;

                case 2:
                    // auxiliary TOP STOP engine is ON
                    rocket.ActivateStopTopEngine();
                    break;

                case 3:
                    // auxiliary TOP LEFT engine is ON
                    rocket.ActivateAuxiliaryTopLeftEngine();
                    break;

                case 4:
                    // auxiliary TOP RIGHT engine is ON
                    rocket.ActivateAuxiliaryTopRightEngine();
                    break;

                case 5:
                    // auxiliary LEFT BOTTOM engine is ON
                    rocket.ActivateAuxiliaryBottomLeftEngine();
                    break;

                case 6:
                    // auxiliary RIGHT BOTTOM engine is ON
                    rocket.ActivateAuxiliaryBottomRightEngine();
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
                Debug.LogFormat("FAILURE: DeathZoneCollision", LOG_TAG);
                EventManager.RaiseSimulationEndEvent(SimulationFinishStatus.FAILURE_DEATH_ZONE_COLLISION);
                GiveDeathZoneCollisionReward();
                EndEpisode();
                return;
            }

            if (simulationState.isPlanetCollisionAccident)
            {
                Debug.LogFormat("FAILURE: PlanetCollision", LOG_TAG);
                EventManager.RaiseSimulationEndEvent(SimulationFinishStatus.FAILURE_PLANET_COLLISION);
                GivePlanetCollisionReward();
                EndEpisode();
                return;
            }

            if (simulationState.IsLandingSucces())
            {
                Debug.LogFormat("SUCCESS: Successfull landing", LOG_TAG);
                EventManager.RaiseSimulationEndEvent(SimulationFinishStatus.SUCCESS);
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
