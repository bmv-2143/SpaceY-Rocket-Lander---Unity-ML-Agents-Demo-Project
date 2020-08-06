using Unity.MLAgents.Policies;
using UnityEngine;

namespace HumbleGames
{
    public class ObjectsActiveStateController : MonoBehaviour
    {
        [SerializeField]
        private BehaviorParameters behaviorParameters;

        [SerializeField]
        private CameraFollow cameraFollow;

        [SerializeField]
        private GameObject[] objsToDeactivateInTrainingMode;
        
        [SerializeField]
        private GameObject[] objsToActivateInTrainingMode;


        private void OnEnable()
        {
            cameraFollow.enabled = IsTrainingMode();

            // if it is not Heuristic mode => deactivate these objects for other modes (training)
            foreach (GameObject go in objsToActivateInTrainingMode)
            {
                go.SetActive(IsTrainingMode());
            }

            foreach (GameObject go in objsToDeactivateInTrainingMode)
            {
                go.SetActive(!IsTrainingMode());
            }
        }

        private bool IsTrainingMode()
        {
            return behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly;
        }
    }
}
