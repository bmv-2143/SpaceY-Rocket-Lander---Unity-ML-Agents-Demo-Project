using HumbleGames.SpaceY.Simulation;

namespace HumbleGames.SpaceY
{
    public class EventManager
    {
        public delegate void SimulationEndAction(SimulationEndStatus status);
        public static event SimulationEndAction OnSimulationEnd;

        public static void RaiseSimulationEndEvent(SimulationEndStatus status)
        {
            OnSimulationEnd?.Invoke(status);
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