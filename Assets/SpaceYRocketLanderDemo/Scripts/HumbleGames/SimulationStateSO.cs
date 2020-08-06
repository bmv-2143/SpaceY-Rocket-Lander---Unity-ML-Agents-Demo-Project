using UnityEngine;

namespace HumbleGames
{
    [CreateAssetMenu(fileName = "SimulationState", menuName = "ScriptableObjects/SimulationState", order = 1)]
    public class SimulationStateSO : ScriptableObject
    {
        public bool isDeathZoneCollisionAccident;
        
        public bool isPlanetCollisionAccident;

        public bool isLeftLegLanded;

        public bool isRightLegLanded;

        public bool IsLandingSucces()
        {
            return isLeftLegLanded && isRightLegLanded;
        }

        public void Reset()
        {
            isDeathZoneCollisionAccident = false;
            isPlanetCollisionAccident = false;
            isLeftLegLanded = false;
            isRightLegLanded = false;
        }
    }
}
