using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(10)]
public class Settings : MonoBehaviour
{
    public Scrollbar scroll;
    public Toggle toggle;
    GameManager gameManager;
    public TMP_Dropdown drop;
    public static Settings instance;

    private void Awake()
    {
        instance ??= this;
    }
    public void Start()
    {
        if (GameManager.Instance == null)
        {
            StartCoroutine(WaitTillLoad());
        }
        else
        {
            StartSet();
            SetVolume();
            SetVSync();
            SetLanguage();
        }
    }
    private IEnumerator WaitTillLoad()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }
        StartSet();
    }
    void StartSet()
    {
        gameManager = GameManager.Instance;
        drop.value = gameManager.language.IndexOf(gameManager.loc);
        scroll.value = gameManager.volume;
        toggle.isOn = Convert.ToBoolean(gameManager.vSync);
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
        gameManager.ChangeSetting(value);
        AudioManager.instance.ChangeVolume(AudioManager.instance.volume);
        AudioListener.volume = scroll.value;
    }

    public void SetVSync()
    {
        string value = "0";
        if (toggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            value = "1";
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
        gameManager.ChangeSetting(sync);

    }
    public void SetLanguage()
    {
        List<string> value = new List<string>()
            {
            "language",
            drop.options[drop.value].text,
            // Додаткові значення списку
            };
        gameManager.ChangeSetting(value);
    }
}
