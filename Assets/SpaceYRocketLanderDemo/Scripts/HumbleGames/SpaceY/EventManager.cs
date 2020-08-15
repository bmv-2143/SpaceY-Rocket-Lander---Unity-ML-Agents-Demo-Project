using HumbleGames.SpaceY.Simulation;
using Unity.MLAgents.Policies;

namespace HumbleGames.SpaceY
{
    public class EventManager
    {

        // ------------------------------------------------------------------------------------------------------------
        //                                       Simulation Events
        // ------------------------------------------------------------------------------------------------------------

        public delegate void SimulationEndAction(SimulationEndStatus status);
        public static event SimulationEndAction OnSimulationEnd;

        public static void RaiseSimulationEndEvent(SimulationEndStatus status)
        {
            OnSimulationEnd?.Invoke(status);
        }

        public delegate void BehaviourTypeChangedAction(BehaviorType behaviourType);
        public static event BehaviourTypeChangedAction OnBehaviourTypeChanged;

        public static void RaiseBehaviourTypeChangedEvent(BehaviorType behaviourType)
        {
            OnBehaviourTypeChanged?.Invoke(behaviourType);
        }

        public delegate void TrainingLargeSetStatsUpdatedAction(float successRate, int numOfSimulations);
        public static event TrainingLargeSetStatsUpdatedAction OnTrainingLargeSetStatsUpdated;

        public static void RaiseTrainingLargeSetStatsUpdatedEvent(float lastLargeSetSuccessRate, int numOfSimulationsInLargeSet)
        {
            OnTrainingLargeSetStatsUpdated?.Invoke(lastLargeSetSuccessRate, numOfSimulationsInLargeSet);
        }

        // ------------------------------------------------------------------------------------------------------------
        //                                          Rocket Events
        // ------------------------------------------------------------------------------------------------------------

        public delegate void RocketMainEngineAction();
        public static event RocketMainEngineAction OnRocketMainEngine;

        public static void RaiseRocketMainEngineEvent()
        {
            OnRocketMainEngine?.Invoke();
        }

        public delegate void RocketAuxEngineAction();
        public static event RocketAuxEngineAction OnRockeAuxEngine;

        public static void RaiseRocketAuxEngineEvent()
        {
            OnRockeAuxEngine?.Invoke();
        }

        public delegate void RocketAccidentAction();
        public static event RocketAccidentAction OnRockeAccident;

        public static void RaiseRocketAccidentEvent()
        {
            OnRockeAccident?.Invoke();
        }
    }

}