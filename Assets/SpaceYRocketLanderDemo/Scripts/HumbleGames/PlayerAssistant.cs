using UnityEngine;

namespace HumbleGames
{
    public class PlayerAssistant : MonoBehaviour
    {

        [SerializeField]
        private GameObject rocket;

        [SerializeField]
        private GameObject planet1;

        [SerializeField]
        private GameObject planet2;

        // Update is called once per frame
        void Update()
        {
            Debug.DrawLine(rocket.transform.position, planet1.transform.position, Color.magenta);
            Debug.DrawLine(rocket.transform.position, planet2.transform.position, Color.magenta);
        }
    }
}
