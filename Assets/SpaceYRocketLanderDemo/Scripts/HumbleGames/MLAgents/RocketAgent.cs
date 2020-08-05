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

        // ------------------------------------------------------------------------------------------------------------
        //                                Unity Lifecycle calls
        // ------------------------------------------------------------------------------------------------------------

        // Update is called once per frame
        private void FixedUpdate()
        {

        }

        // ------------------------------------------------------------------------------------------------------------
        //                                ML-Agents related calls
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initial setup, called when the agent is enabled
        /// </summary>
        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reset the agent and area
        /// </summary>
        public override void OnEpisodeBegin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Collect all non-Raycast observations
        /// </summary>
        public override void CollectObservations(VectorSensor sensor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform actions based on a vector of numbers
        /// </summary>
        /// <param name="vectorAction">The list of actions to take</param>
        public override void OnActionReceived(float[] vectorAction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read inputs from the keyboard and convert them to a list of actions.
        /// This is called only when the player wants to control the agent and has set
        /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
        /// </summary>
        /// <returns>A vectorAction array of floats that will be passed into <see cref="AgentAction(float[])"/></returns>
        public override void Heuristic(float[] actionsOut)
        {
            throw new NotImplementedException();
        }
    }
}
