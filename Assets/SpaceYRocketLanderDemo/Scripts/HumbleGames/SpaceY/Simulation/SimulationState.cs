using UnityEngine;

namespace HumbleGames.SpaceY.Simulation
{
    public class SimulationState : MonoBehaviour
    {
        private readonly string LOG_TAG = typeof(SimulationState).Name;

        public bool isDeathZoneCollisionAccident;
        public bool isPlanetCollisionAccident;

        public bool isLeftLegLandedOnTargetPlanet;
        public bool isRightLegLandedOnTargetPlanet;

        public bool isLeftLegLandedOnBasePlanet;
        public bool isRightLegLandedOnBasePlanet;

        public bool AreBothLegsOnTargetPlanet()
        {
            return isLeftLegLandedOnTargetPlanet && isRightLegLandedOnTargetPlanet;
        }

        public void Reset()
        {
            isDeathZoneCollisionAccident = false;
            isPlanetCollisionAccident = false;
            isLeftLegLandedOnTargetPlanet = false;
            isRightLegLandedOnTargetPlanet = false;
            isLeftLegLandedOnBasePlanet = false;
            isRightLegLandedOnBasePlanet = false;
    }

        public void DebugPrint(string prefix = "")
        {
            Debug.LogFormat(prefix + ": {0}: deathZoneCol: {1},  planetCol: {2},  leftLandTg: {3},  rightLandTg: {4}, " +
                            "leftLandBs: {5},  rightLandBs: {6}", 
                            LOG_TAG,
                            isDeathZoneCollisionAccident,
                            isPlanetCollisionAccident,
                            isLeftLegLandedOnTargetPlanet,
                            isRightLegLandedOnTargetPlanet,
                            isLeftLegLandedOnBasePlanet,
                            isRightLegLandedOnBasePlanet);
        }
    }
}