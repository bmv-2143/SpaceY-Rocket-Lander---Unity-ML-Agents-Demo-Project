using System.Collections;
using UnityEngine;

namespace HumbleGames
{
    public class HideTitle : MonoBehaviour
    {

        [SerializeField]
        private GameObject titleTxtGo;

        void Start()
        {
            StartCoroutine(HideTitleAfterTimeout());
        }

        private IEnumerator HideTitleAfterTimeout()
        {
            yield return new WaitForSeconds(1.5f);
            titleTxtGo.SetActive(false);
        }
    }
}
