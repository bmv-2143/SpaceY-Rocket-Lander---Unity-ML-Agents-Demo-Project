using HumbleGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumbleGames
{
    public class SimulationStatsTracker : MonoBehaviour
    {
        private readonly string LOG_TAG = typeof(SimulationStatsTracker).Name;

        private int totalSimulations;
        private int successCount;
        private int failCount;
        private float successRate;

        private void OnEnable()
        {
            EventManager.OnSimulationEnd += OnSimulationEnd;
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
                successCount++;
            }

            else
            {
                failCount++;
            }

            successRate = 100f * successCount / totalSimulations;

            Debug.LogFormat("{0}: totalSimulations: {1}, succesRate: {2}%, successCount: {3}, failCount: {4}",
                            LOG_TAG,
                            totalSimulations,
                            successRate,
                            successCount,
                            failCount);
        }
    }
}