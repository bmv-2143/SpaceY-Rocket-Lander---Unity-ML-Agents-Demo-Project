using UnityEngine;

namespace HumbleGames
{
    [CreateAssetMenu(fileName = "TagHolder", menuName = "ScriptableObjects/TagHolder", order = 1)]
    public class TagHolder : ScriptableObject
    {
        public string deathZone = "DeathZone";
        public string legLeftLandingProbe = "LegLeftLandingProbe";
        public string legRightLandingProbe = "LegRightLandingProbe";
        public string targetPlanet = "TargetPlanet";
        public string basePlanet = "BasePlanet";
    }
}
