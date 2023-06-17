using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
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
        List<string> value = new List<string>()
        {
            "volume", 
            scroll.value.ToString(),
            // Додаткові значення списку
        };
        volume.ChangeSetting(value);
        AudioListener.volume = scroll.value;
    }
    public void SetVSync()
    {
        string value = "0";
        if (toggle.isOn)
        {
            value = "1";
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            
        }
        else
        {
            value = "0";
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
        }
        List<string> sync = new List<string>()
        {
            "v-sync",
            value,
            // Додаткові значення списку
        };
        volume.ChangeSetting(sync);
    }
}
