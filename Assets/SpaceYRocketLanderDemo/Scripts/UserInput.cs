using UnityEngine;

public class UserInput : MonoBehaviour {

	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
