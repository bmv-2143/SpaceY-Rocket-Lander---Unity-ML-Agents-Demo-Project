using HumbleGames.SpaceY.MLAgents;
using HumbleGames.SpaceY.Simulation;
using System;
using System.Collections;
using System.Text;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.UI;

namespace HumbleGames.SpaceY.UI
{
    public class UIManager : MonoBehaviour
    {
        private readonly string LOG_TAG = typeof(UIManager).Name;

        [SerializeField]
        private SimulationConfig simulationConfig;

        [SerializeField]
        private GameObject musicHolderObject;
        
        [SerializeField]
        private Text musicButtonText;

        [SerializeField]
        private Text changeBehaviourTypeButtonText;

        [SerializeField]
        private GameObject simulationTitle;

        [SerializeField]
        private string successMessage = "Success!";

        [SerializeField]
        private string failureMessagePrefix = "Failure: ";

        [SerializeField]
        private float autoHideTextDelay = 1.5f;

        [SerializeField]
        private Text successTextView;
        
        [SerializeField]
        private Text failureTextView;

        [SerializeField]
        private Text totalSimulationsText;
        
        [SerializeField]
        private Text totalSuccessText;
        
        [SerializeField]
        private Text totalFailText;
        
        [SerializeField]
        private Text totalSuccessRateText;

        private StringBuilder totalSimulationsSb;
        private StringBuilder totalSuccessRateSb;
        private StringBuilder totalSuccessSb;
        private StringBuilder totalFailSb;

        private string totalSimulationsPrefix;
        private string totalSuccessPrefix;
        private string totalFailPrefix;
        private string totalSuccessRatePrefix;

        private void Awake()
        {
            successTextView.gameObject.SetActive(false);
            failureTextView.gameObject.SetActive(false);

            totalSimulationsPrefix = totalSimulationsText.text;
            totalSuccessPrefix     = totalSuccessText.text;
            totalFailPrefix     = totalFailText.text;
            totalSuccessRatePrefix     = totalSuccessRateText.text;

            totalSimulationsSb = new StringBuilder();
            totalSuccessRateSb = new StringBuilder();
            totalSuccessSb     = new StringBuilder();
            totalFailSb        = new StringBuilder();

            OnTotalStatsUpdated(0, 0, 0, 0);
        }

        private void OnEnable()
        {
            if (simulationConfig.simulationMode == SimulationConfig.SimulationMode.Demo)
            {
                EventManager.OnSimulationEnd += OnSimulationEnd;
            }

            EventManager.OnTrainingTotalStatsUpdated += OnTotalStatsUpdated;
            StartCoroutine(ShowThenHideObject(simulationTitle, autoHideTextDelay));
        }

        private void OnDisable()
        {
            if (simulationConfig.simulationMode == SimulationConfig.SimulationMode.Demo)
            {
                EventManager.OnSimulationEnd -= OnSimulationEnd;
            }
            
            EventManager.OnTrainingTotalStatsUpdated -= OnTotalStatsUpdated;
        }

        public void OnButtonSwitchBehaviorType()
        {
            UpdateSwitchBehaviorTypeButtonText();
        }

        public void OnButtonSwitchMusic()
        {
            musicHolderObject.SetActive(!musicHolderObject.activeSelf);

            musicButtonText.text = musicHolderObject.activeSelf ? "Music: ON" : "Music: OFF";
        }

        public void OnButtonSetResolution1280x720()
        {
            Display.displays[0].SetRenderingResolution(1280, 720);
        }

        public void OnButtonSetResolution1920x1080()
        {
            Display.displays[0].SetRenderingResolution(1920, 1080);
        }

        private void UpdateSwitchBehaviorTypeButtonText()
        {
            if ((simulationConfig.SimulationBehaviourType == BehaviorType.Default) ||
                (simulationConfig.SimulationBehaviourType == BehaviorType.InferenceOnly))
            {
                simulationConfig.SimulationBehaviourType = BehaviorType.HeuristicOnly;
                changeBehaviourTypeButtonText.text = "Player control";
            }

            else if (simulationConfig.SimulationBehaviourType == BehaviorType.HeuristicOnly)
            {
                simulationConfig.SimulationBehaviourType = BehaviorType.Default;
                changeBehaviourTypeButtonText.text = "ML-Agent control";
            }
        }

        private void OnSimulationEnd(SimulationEndStatus status)
        {
            if (status == SimulationEndStatus.SUCCESS)
            {
                ShowEndGameText(successTextView, successMessage);
            }

            else
            {
                ShowEndGameText(failureTextView, GetFailureMessage(status));
            }
        }

        private void OnTotalStatsUpdated(int totalSimulations, int totalSuccess, int totalFailures, float totalSuccessRate)
        {
            totalSimulationsSb.Clear();
            totalSuccessSb.Clear();
            totalFailSb.Clear();
            totalSuccessRateSb.Clear();

            totalSimulationsText.text = 
                totalSimulationsSb.Append(totalSimulationsPrefix).Append(totalSimulations).ToString();
            totalSuccessText.text = totalSuccessSb.Append(totalSuccessPrefix).Append(totalSuccess).ToString();
            totalFailText.text = totalFailSb.Append(totalFailPrefix).Append(totalFailures).ToString();
            totalSuccessRateText.text = totalSuccessRateSb.
                                        Append(totalSuccessRatePrefix).
                                        Append(Math.Round(totalSuccessRate, 2)).
                                        Append("%").ToString();
        }

        private void ShowEndGameText(Text textView, string message)
        {
            textView.text = message;
            StartCoroutine(ShowThenHideObject(textView.gameObject, autoHideTextDelay));
        }

        private IEnumerator ShowThenHideObject(GameObject targetObject, float delay)
        {
            targetObject.SetActive(true);
            yield return new WaitForSeconds(delay);
            targetObject.SetActive(false);
        }

        private string GetFailureMessage(SimulationEndStatus status)
        {
            if (status == SimulationEndStatus.FAILURE_PLANET_COLLISION)
            {
                return failureMessagePrefix + "Planet Collision";
            }

            else if (status == SimulationEndStatus.FAILURE_DEATH_ZONE_COLLISION)
            {
                return failureMessagePrefix + "DeathZone Collision";
            }

            else
            {
                Debug.LogFormat("{0}: GetFailureMessage: unexpected SimulationFinishStatus: {1}", LOG_TAG, status);
                return null;
            }
        }
    }
}
