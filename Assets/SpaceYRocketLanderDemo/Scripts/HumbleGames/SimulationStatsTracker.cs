using HumbleGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HumbleGames
{
    public class SimulationStatsTracker : MonoBehaviour
    {
        private readonly string LOG_TAG = typeof(SimulationStatsTracker).Name;

        [SerializeField]
        private int numLastSimsToTrack = 100;
       
        private int[] resultsOfLastTrackedSims;
        private int lastTrackedSimulationsIndex;

        private int totalSimulations;
        private int totalSuccessCount;
        private int totalFailCount;
        private float totalSuccessRate;


        private void OnEnable()
        {
            EventManager.OnSimulationEnd += OnSimulationEnd;

            resultsOfLastTrackedSims = new int[numLastSimsToTrack];
        }

        private void OnDisable()
        {
            EventManager.OnSimulationEnd -= OnSimulationEnd;
        }

        private void OnSimulationEnd(SimulationEndStatus status)
        {
            totalSimulations++;

            if (status == SimulationEndStatus.SUCCESS)
            {
                totalSuccessCount++;
            }

            else
            {
                totalFailCount++;
            }

            totalSuccessRate = 100f * totalSuccessCount / totalSimulations;

            UpdateLastSimulationsSet(status);

            if (totalSimulations % 100 == 0)
            {
                Debug.LogFormat("{0}: totalSimulations: {1},  " +
                                "successRateOver[{2}]: {3}% " +
                                "totalSuccesRate: {4}%, " +
                                "totalSuccessCount: {5}, " +
                                "totalFailCount: {6}",
                                LOG_TAG, 
                                totalSimulations,
                                numLastSimsToTrack,
                                GetSuccessRateOfLastSimulationsSet(),
                                totalSuccessRate,
                                totalSuccessCount,
                                totalFailCount);
            }
        }

        private float GetSuccessRateOfLastSimulationsSet()
        {
            return 100f * resultsOfLastTrackedSims.Sum() / numLastSimsToTrack;
        }

        private void UpdateLastSimulationsSet(SimulationEndStatus status)
        {
            if (status == SimulationEndStatus.SUCCESS)
            {
                resultsOfLastTrackedSims[lastTrackedSimulationsIndex] = 1;
            }
            else
            {
                resultsOfLastTrackedSims[lastTrackedSimulationsIndex] = 0;
            }

            lastTrackedSimulationsIndex++;

            if (lastTrackedSimulationsIndex >= numLastSimsToTrack)
            {
                lastTrackedSimulationsIndex = 0;
            }
        }
    }
}