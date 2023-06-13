using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    //public AudioListener mixer;
    public Scrollbar scroll;
    public Toggle toggle;
    public LocalizationManager volume;

    public void Start()
    {
        scroll.value = volume.volume;
        AudioListener.volume = scroll.value;
    }
    // Метод для зміни гучності гри
    public void SetVolume()
    {
        volume.ChangeVolume(scroll.value);
        AudioListener.volume = scroll.value;
    }
    public void SetVSync()
    {
        if (toggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
        }
    }
}
