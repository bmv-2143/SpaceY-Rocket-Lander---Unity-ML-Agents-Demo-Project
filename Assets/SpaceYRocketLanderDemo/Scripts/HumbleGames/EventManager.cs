
namespace HumbleGames
{
    public class EventManager
    {
        public delegate void SimulationEndAction(SimulationFinishStatus status);
        public static event SimulationEndAction OnSimulationEnd;

        public static void RaiseSimulationEndEvent(SimulationFinishStatus status)
        {
            OnSimulationEnd?.Invoke(status);
        }
    }

}