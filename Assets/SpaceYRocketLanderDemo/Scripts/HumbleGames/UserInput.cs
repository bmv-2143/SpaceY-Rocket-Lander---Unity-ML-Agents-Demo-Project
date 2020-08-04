using UnityEngine;

namespace HumbleGames
{
    public class UserInput : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}