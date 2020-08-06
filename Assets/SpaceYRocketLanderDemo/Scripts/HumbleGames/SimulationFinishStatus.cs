using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumbleGames
{
    public enum SimulationFinishStatus 
    {
        SUCCESS,
        FAILURE_PLANET_COLLISION,
        FAILURE_DEATH_ZONE_COLLISION
    }
}