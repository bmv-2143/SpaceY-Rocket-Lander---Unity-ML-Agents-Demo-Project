using UnityEditor;
using UnityEngine;

public class SpaceYMenu : EditorWindow
{

    [MenuItem("Tools/SpaceY/Reset Playerprefs")]

    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

}