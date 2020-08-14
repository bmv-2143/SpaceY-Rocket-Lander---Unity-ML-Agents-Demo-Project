using HumbleGames.SpaceY.MLAgents;
using HumbleGames.SpaceY.Simulation;
using System.Collections;
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

        private void Awake()
        {
            successTextView.gameObject.SetActive(false);
            failureTextView.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (simulationConfig.simulationMode == SimulationConfig.SimulationMode.Demo)
            {
                EventManager.OnSimulationEnd += OnSimulationEnd;
            }

            StartCoroutine(ShowThenHideObject(simulationTitle, autoHideTextDelay));
        }

        private void OnDisable()
        {
            if (simulationConfig.simulationMode == SimulationConfig.SimulationMode.Demo)
            {
                EventManager.OnSimulationEnd -= OnSimulationEnd;
            }
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
