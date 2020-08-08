using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumbleGames.Simulation
{
    public class SimulationState : MonoBehaviour
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