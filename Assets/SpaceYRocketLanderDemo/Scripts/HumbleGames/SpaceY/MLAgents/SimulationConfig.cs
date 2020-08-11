using Unity.MLAgents.Policies;
using UnityEngine;

namespace HumbleGames.SpaceY.MLAgents
{
    [CreateAssetMenu(fileName = "SimulationConfig", menuName = "ScriptableObjects/SimulationConfig", order = 1)]
    public class SimulationConfig : ScriptableObject
    {
        // Is expected to be changed in Editor in Edit Mode only
        public SimulationMode simulationMode;

        // Can be switched from the UI at runtime
        private BehaviorType behaviourType;

        public BehaviorType SimulationBehaviourType
        {
            get
            {
                return behaviourType;
            }

            set 
            {
                behaviourType = value;
                EventManager.RaiseBehaviourTypeChangedEvent(behaviourType);
            }
        }

        public enum SimulationMode
        {
            /// <summary>
            /// A user plays the game or watches how ML-Agent plays.
            /// </summary>
            Demo,

            /// <summary>
            /// Training is performed in Unity Editor.
            /// </summary>
            Training
        }
    }
}
