using UnityEngine;

namespace HumbleGames
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
    }
}
