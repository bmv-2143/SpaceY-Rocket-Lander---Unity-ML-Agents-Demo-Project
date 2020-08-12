using HumbleGames.SpaceY.MLAgents;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace HumbleGames.SpaceY.Simulation
{
    public class SuccessBasedCurriculumTrainer : MonoBehaviour
    {
        private readonly string LOG_TAG = typeof(SuccessBasedCurriculumTrainer).Name;

        [SerializeField]
        private bool setTrainingParamsInAwake;

        [SerializeField]
        private SuccessRateToTrainingParams[] successRatesToParams;

        private SimulationArea[] simulationAreas;

        private int activeTrainingParamsIndex;

        private void Awake()
        {
            simulationAreas = GetComponentsInChildren<SimulationArea>();

            Assert.IsTrue(simulationAreas.Length > 0);

            if (setTrainingParamsInAwake)
            {
                // activate the first set of simulation params
                SetSimulationiTrainingParams(successRatesToParams[0].simulationParams);
            }
        }

        private void OnEnable()
        {
            EventManager.OnTrainingSuccessRateAchieved += OnTrainingSuccessRateAchieved;
        }

        private void OnDisable()
        {
            EventManager.OnTrainingSuccessRateAchieved -= OnTrainingSuccessRateAchieved;
        }

        private void OnTrainingSuccessRateAchieved(float achievedSuccessRate, int numOfSimulations)
        {
            if (successRatesToParams[activeTrainingParamsIndex].successRate <= achievedSuccessRate)
            {
                activeTrainingParamsIndex++;

                if (activeTrainingParamsIndex > successRatesToParams.Length - 1)
                {
                    Debug.LogFormat("{0}: No more training params to set", LOG_TAG);
                }

                else
                {
                    Debug.LogFormat("{0}: Setting training params at index: {1}", LOG_TAG, activeTrainingParamsIndex);
                    SetSimulationiTrainingParams(successRatesToParams[activeTrainingParamsIndex].simulationParams);
                }
            }
        }

        private void SetSimulationiTrainingParams(SimulationTrainingParams simulationTrainingParams)
        {
            foreach (SimulationArea simArea in simulationAreas)
            {
                simArea.SetSimulationTrainingParams(simulationTrainingParams);
            }
        }

        [Serializable]
        public struct SuccessRateToTrainingParams
        {
            /// <summary>
            /// Success rate at which to switch to the next TrainingParams (to the next index in the array).
            /// </summary>
            public float successRate;

            public SimulationTrainingParams simulationParams;
        }

        [Serializable]
        public struct SimulationTrainingParams
        {
            public float deathZoneWidth;
            public float deathZoneHeight;

            [Space]

            public float planetDesignationAreaWidth;
            public float planetDesignationAreaHeight;
            
            [Space]

            public float rocketToTargetPlanetMinOffset;
            public float rocketToTargetPlanetMaxOffset;

            [Space]

            public float rocketSpawnNearTargetPlanetProbability;
        }
    }
}