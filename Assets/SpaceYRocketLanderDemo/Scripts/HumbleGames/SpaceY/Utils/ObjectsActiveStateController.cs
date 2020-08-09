using Unity.MLAgents.Policies;
using UnityEngine;

namespace HumbleGames.SpaceY.Utils
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

        [SerializeField]
        private string tagToActivateObjsInTrainingMode = "ActivateWhileTraining";
        
        [SerializeField]
        private string tagToDeactivateObjsInTrainingMode = "DeactivateWhileTraining";

        private void OnEnable()
        {
            cameraFollow.enabled = !IsTrainingMode();

            // if it is not Heuristic mode => deactivate these objects for other modes (training)
            foreach (GameObject go in objsToActivateInTrainingMode)
            {
                go.SetActive(IsTrainingMode());
            }

            foreach (GameObject go in objsToDeactivateInTrainingMode)
            {
                go.SetActive(!IsTrainingMode());
            }

            ProcessTaggedObjects();
        }

        private bool IsTrainingMode()
        {
            return behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly;
        }

        private void ProcessTaggedObjects()
        {
            GameObject[] objsToActivateWhileTraining = GameObject.FindGameObjectsWithTag(tagToActivateObjsInTrainingMode);

            foreach (GameObject go in objsToActivateWhileTraining)
            {
                go.SetActive(IsTrainingMode());
            }

            GameObject[] objsToDeactivateWhileTraining = GameObject.FindGameObjectsWithTag(tagToDeactivateObjsInTrainingMode);

            foreach (GameObject go in objsToDeactivateWhileTraining)
            {
                go.SetActive(!IsTrainingMode());
            }
        }
    }
}
