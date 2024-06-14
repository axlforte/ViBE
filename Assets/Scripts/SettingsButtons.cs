using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtons : MonoBehaviour
{
    private bool fullscreened;

    public GameObject Resolution, Framerate, Input;

    public void ChangeSettings(){
        if(Resolution.activeSelf){
            Resolution.SetActive(false);
            Framerate.SetActive(true);
            Input.SetActive(false);
        } else if(Framerate.activeSelf){
            Resolution.SetActive(false);
            Framerate.SetActive(false);
            Input.SetActive(true);
        } else {
            Resolution.SetActive(true);
            Framerate.SetActive(false);
            Input.SetActive(false);
        }
    }

    public void ChangeSettingsInReverse(){
        if(Resolution.activeSelf){
            Resolution.SetActive(false);
            Framerate.SetActive(false);
            Input.SetActive(true);
        } else if(Input.activeSelf){
            Resolution.SetActive(false);
            Framerate.SetActive(true);
            Input.SetActive(false);
        } else {
            Resolution.SetActive(true);
            Framerate.SetActive(false);
            Input.SetActive(false);
        }
    }

    public void SNES()
    {
        Screen.SetResolution(256, 224, fullscreened);
    }

    public void PSX()
    {
        Screen.SetResolution(640, 480, fullscreened);
    }

    public void PS2()
    {
        Screen.SetResolution(1280, 720, fullscreened);
    }

    public void HD()
    {
        Screen.SetResolution(1920, 1080, fullscreened);
    }

    public void FourK()
    {
        Screen.SetResolution(3840, 2160, fullscreened);
    }

    public void ToggleFullscreened()
    {
        fullscreened = !fullscreened;
    }

    public void ChangeFramerate(int rate)
    {
        Application.targetFrameRate = rate;
    }

    public void SetVSync()
    {
        if (QualitySettings.vSyncCount == 1)
        {
            QualitySettings.vSyncCount = 0;
        } else
        {
            QualitySettings.vSyncCount = 1;
        }
    }
}
