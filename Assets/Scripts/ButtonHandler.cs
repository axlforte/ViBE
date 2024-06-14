using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonHandler : MonoBehaviour
{

    public DetailsPanel dp;
    public int index;
    public string levelName, levelFilePath;

    //public TMP_InputField inputField;

    public void Click(){
        dp.ButtonPress(index);
    }

    public void GoToPlayMode()
    {
        PlayerPrefs.SetString("Level To Load", levelName);
        PlayerPrefs.SetString("Level File Path", levelFilePath);
        SceneManager.LoadScene("PlayScene");
    }

    public void GoToPlayMode(TMP_InputField inputField)
    {
        PlayerPrefs.SetString("Level To Load", inputField.text);
        PlayerPrefs.SetString("Level File Path", "None");
        SceneManager.LoadScene("PlayScene");
    }
}
