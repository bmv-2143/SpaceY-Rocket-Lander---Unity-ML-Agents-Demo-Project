using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HumbleGames.UI
{
    public class UIManager : MonoBehaviour
    {
        private readonly string LOG_TAG = typeof(UIManager).Name;

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
            EventManager.OnSimulationEnd += OnSimulationEnd;
            StartCoroutine(ShowThenHideObject(simulationTitle, autoHideTextDelay));
        }

        private void OnDisable()
        {
            EventManager.OnSimulationEnd -= OnSimulationEnd;
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
