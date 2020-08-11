using HumbleGames.SpaceY.MLAgents;
using System;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace HumbleGames.SpaceY.Utils
{
    public class ObjectsActiveStateController : MonoBehaviour
    {
        [SerializeField]
        private SimulationConfig simulationConfig;

        [SerializeField]
        private string tagToActivateObjsInTrainingMode = "ActivateWhileTraining";
        
        [SerializeField]
        private string tagToDeactivateObjsInTrainingMode = "DeactivateWhileTraining";

        [Space]

        [SerializeField]
        private GameObject[] objsToDeactivateInTrainingMode;
        
        [SerializeField]
        private GameObject[] objsToActivateInTrainingMode;


        GameObject[] objsToActivateWhileTraining;
        GameObject[] objsToDeactivateWhileTraining;

        private void OnEnable()
        {
            objsToActivateWhileTraining = GameObject.FindGameObjectsWithTag(tagToActivateObjsInTrainingMode);
            objsToDeactivateWhileTraining = GameObject.FindGameObjectsWithTag(tagToDeactivateObjsInTrainingMode);

            UpdateObjectsAndComponentsState();
            EventManager.OnBehaviourTypeChanged += OnBehaviourChanged;
        }

        private void OnDisable()
        {
            EventManager.OnBehaviourTypeChanged -= OnBehaviourChanged;
        }

        private void UpdateObjectsAndComponentsState()
        {
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
            //return simulationConfig.SimulationBehaviourType != BehaviorType.HeuristicOnly;
            return simulationConfig.simulationMode == SimulationConfig.SimulationMode.Training;
        }

        private void ProcessTaggedObjects()
        {
            foreach (GameObject go in objsToActivateWhileTraining)
            {
                go.SetActive(IsTrainingMode());
            }

            foreach (GameObject go in objsToDeactivateWhileTraining)
            {
                go.SetActive(!IsTrainingMode());
            }
        }

        private void OnBehaviourChanged(BehaviorType behaviourType)
        {
            UpdateObjectsAndComponentsState();
        }
    }
}
