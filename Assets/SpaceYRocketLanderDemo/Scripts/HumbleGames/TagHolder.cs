using UnityEngine;

namespace HumbleGames
{
    [CreateAssetMenu(fileName = "TagHolder", menuName = "ScriptableObjects/TagHolder", order = 1)]
    public class TagHolder : ScriptableObject
    {
        public string deathZone = "DeathZone";
    }
}
