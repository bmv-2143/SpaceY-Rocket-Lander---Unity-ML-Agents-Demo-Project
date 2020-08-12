using UnityEngine;

namespace HumbleGames.SpaceY.MLAgents
{
    [CreateAssetMenu(fileName = "RewardConfig", menuName = "ScriptableObjects/RewardConfig", order = 1)]
    public class RewardConfig : ScriptableObject
    {
        public float landingSuccessReward = 1f;
        public float deathZoneCollisionReward = -1f;
        public float planetCollisionReward = -1f;
        public float legProbeTouchesBasePlanetReward = -0.001f;

        /// <summary>
        /// Used for calculation of stepwise reward of RocketAgent.
        /// </summary>
        public float agentStepwiseRewardBase = -1f;

        /// <summary>
        /// A number of rocket's steps (fixed updates) to stay on target planet with both legs to consider that the 
        /// landing is successfull.
        /// </summary>
        public int stepsOnPlanetForSuccess = 12;
        
        public float bothLegsOnPlanetStepwiseReward = 0.02f;

    }
}
