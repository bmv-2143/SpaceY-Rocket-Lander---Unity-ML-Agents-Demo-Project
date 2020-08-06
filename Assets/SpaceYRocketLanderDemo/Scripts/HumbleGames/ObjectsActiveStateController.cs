using Unity.MLAgents.Policies;
using UnityEngine;

namespace HumbleGames
{
    public class ObjectsActiveStateController : MonoBehaviour
    {
        [SerializeField]
        private BehaviorParameters behaviorParameters;

        [SerializeField]
        private GameObject[] objsDeactivateInTrainingMode;

        private void OnEnable()
        {
            // if it is not Heuristic mode => deactivate these objects for other modes (training)
            foreach (GameObject go in objsDeactivateInTrainingMode)
            {
                go.SetActive(ShouldObjectBeActive());
            }
        }

        private bool ShouldObjectBeActive()
        {
            return behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly;
        }
    }
}
