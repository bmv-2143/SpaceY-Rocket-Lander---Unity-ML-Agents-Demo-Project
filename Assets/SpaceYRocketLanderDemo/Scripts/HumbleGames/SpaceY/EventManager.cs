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
    }

}