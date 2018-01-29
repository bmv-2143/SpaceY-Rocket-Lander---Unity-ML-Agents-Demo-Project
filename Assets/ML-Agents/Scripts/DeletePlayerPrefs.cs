using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletePlayerPrefs : MonoBehaviour {

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
    }
}
