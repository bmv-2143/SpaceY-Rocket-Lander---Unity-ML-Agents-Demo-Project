using UnityEngine;

namespace HumbleGames.SpaceY.Utils
{
    /// <summary>
    /// Draws lines from the rocket to the planets.
    /// </summary>
    public class DrawLinesToPlanets : MonoBehaviour
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
            Debug.DrawLine(rocket.transform.position, planet1.transform.position, Color.blue);
            Debug.DrawLine(rocket.transform.position, planet2.transform.position, Color.green);
        }
    }
}
