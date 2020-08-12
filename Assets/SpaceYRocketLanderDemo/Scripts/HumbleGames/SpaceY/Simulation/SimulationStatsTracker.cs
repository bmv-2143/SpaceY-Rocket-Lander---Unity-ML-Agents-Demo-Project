﻿using System.Linq;
using UnityEngine;

namespace HumbleGames.SpaceY.Simulation
{
    public class SimulationStatsTracker : MonoBehaviour
    {
        private readonly string LOG_TAG = typeof(SimulationStatsTracker).Name;

        [SerializeField]
        private int numLastSimsToTrackSmallSet = 100;
        
        [SerializeField]
        private int numLastSimsToTrackLargeSet = 1000;
       
        private int[] resultsOfLastTrackedSimsSmallSet;
        private int[] resultsOfLastTrackedSimsLargeSet;
        private int lastTrackedSimsSmallSetIndex;
        private int lastTrackedSimsLargeSetIndex;

        private int totalSimulations;
        private int totalSuccessCount;
        private int totalFailCount;
        private float totalSuccessRate;


        private void OnEnable()
        {
            EventManager.OnSimulationEnd += OnSimulationEnd;

            resultsOfLastTrackedSimsSmallSet = new int[numLastSimsToTrackSmallSet];
            resultsOfLastTrackedSimsLargeSet = new int[numLastSimsToTrackLargeSet];
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

            UpdateLastSimulationSets(status);

            if (totalSimulations % numLastSimsToTrackSmallSet == 0)
            {
                Debug.LogFormat("{0}: SMALL: totalSimulations: {1},  " +
                                "successRateOver[{2}]: {3}% " +
                                "totalSuccesRate: {4}%, " +
                                "totalSuccessCount: {5}, " +
                                "totalFailCount: {6}",
                                LOG_TAG,
                                totalSimulations,
                                numLastSimsToTrackSmallSet,
                                GetSuccessRateOfLastSimsSmallSet(),
                                totalSuccessRate,
                                totalSuccessCount,
                                totalFailCount);
            }

            if (totalSimulations % numLastSimsToTrackLargeSet == 0)
            {
                Debug.LogFormat("{0}: LARGE: totalSimulations: {1},  " +
                                "successRateOver[{2}]: {3}% " +
                                "totalSuccesRate: {4}%, " +
                                "totalSuccessCount: {5}, " +
                                "totalFailCount: {6}",
                                LOG_TAG,
                                totalSimulations,
                                numLastSimsToTrackLargeSet,
                                GetSuccessRateOfLastSimsLargeSet(),
                                totalSuccessRate,
                                totalSuccessCount,
                                totalFailCount);
            }
        }

        private float GetSuccessRateOfLastSimsSmallSet()
        {
            return 100f * resultsOfLastTrackedSimsSmallSet.Sum() / numLastSimsToTrackSmallSet;
        }

        private float GetSuccessRateOfLastSimsLargeSet()
        {
            // TODO: refactor, don't sum large sets each time
            return 100f * resultsOfLastTrackedSimsLargeSet.Sum() / numLastSimsToTrackLargeSet;
        }

        private void UpdateLastSimulationSets(SimulationEndStatus status)
        {
            if (status == SimulationEndStatus.SUCCESS)
            {
                resultsOfLastTrackedSimsSmallSet[lastTrackedSimsSmallSetIndex] = 1;
                resultsOfLastTrackedSimsLargeSet[lastTrackedSimsLargeSetIndex] = 1;
            }
            else
            {
                resultsOfLastTrackedSimsSmallSet[lastTrackedSimsSmallSetIndex] = 0;
                resultsOfLastTrackedSimsLargeSet[lastTrackedSimsLargeSetIndex] = 0;
            }

            lastTrackedSimsSmallSetIndex++;
            lastTrackedSimsLargeSetIndex++;

            if (lastTrackedSimsSmallSetIndex >= numLastSimsToTrackSmallSet)
            {
                lastTrackedSimsSmallSetIndex = 0;
            }

            if (lastTrackedSimsLargeSetIndex >= numLastSimsToTrackLargeSet)
            {
                lastTrackedSimsLargeSetIndex = 0;
            }
        }
    }
}