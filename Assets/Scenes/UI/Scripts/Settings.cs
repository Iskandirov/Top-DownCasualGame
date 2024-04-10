using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class Settings : MonoBehaviour
{
    public Slider volumeMusic;
    public Slider volumeSFX;
    public Image muteVolume;
    public Image muteSFX;
    public Toggle toggle;
    GameManager gameManager;
    public TMP_Dropdown drop;
    public TextMeshProUGUI volumeValueTxt;
    public TextMeshProUGUI sfxValueTxt;
    public static Settings instance;
    [Header("Resolution")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    List<Resolution> filtredResolutions;
    RefreshRate currentRefreshRate;
    int currentResolutionIndex = 0;
    public Dictionary<int, int> resolutionVariations = new Dictionary<int, int>()
{
    { 1280, 720 },
    { 1920, 1080 },
    { 2560, 1440 },
    { 3840, 2160 }
};
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
            SetVolumeMusic();
            SetVolumeSFX();
            SetVSync();
            SetLanguage();
            SetResolutionStart();
        }
    }
    void SetResolutionStart()
    {
        filtredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        foreach (var screenSize in resolutionVariations)
        {
            Resolution newRes = new Resolution();
            newRes.width = screenSize.Key;
            newRes.height = screenSize.Value;
            filtredResolutions.Add(newRes);

            // Set current resolution if it matches
            if (screenSize.Key == Screen.width && screenSize.Value == Screen.height)
            {
                currentResolutionIndex = filtredResolutions.Count - 1;
            }
        }
        List<string> options = new List<string>();
        for (int i = 0; i < filtredResolutions.Count; i++)
        {
            string resolutionOption = filtredResolutions[i].width + "x" + filtredResolutions[i].height;
            options.Add(resolutionOption);
            if (filtredResolutions[i].width == Screen.width && filtredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filtredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
        GameManager.SaveResolution(resolution);
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
        toggle.isOn = Convert.ToBoolean(gameManager.vSync);
        volumeMusic.value = gameManager.volumeMusic;
        volumeSFX.value = gameManager.volumeSFX;
        AudioManager.instance.musicObj.volume = volumeMusic.value;
        AudioManager.instance.sfxObj.volume = volumeSFX.value;
        muteVolume.gameObject.SetActive(volumeMusic.value == 0 ? true : false);
        muteSFX.gameObject.SetActive(volumeSFX.value == 0 ? true : false);
    }

    // Метод для зміни гучності гри
    public void SetVolumeMusic()
    {
        List<string> value = new List<string>()
            {
            "volume",
            volumeMusic.value.ToString(),
            // Додаткові значення списку
            };
       
        gameManager.ChangeSetting(value);
        AudioManager.instance.ChangeVolume(AudioManager.instance.volumeMusic, AudioManager.instance.musicObj);
        muteVolume.gameObject.SetActive(volumeMusic.value == 0 ? true : false);
        volumeValueTxt.text = (AudioManager.instance.volumeMusic * 100).ToString("0.") + "%";
    }
    public void SetVolumeSFX()
    {
       
        List<string> valueSFX = new List<string>()
            {
            "sfx",
            volumeSFX.value.ToString(),
            // Додаткові значення списку
            };
        gameManager.ChangeSetting(valueSFX);
        AudioManager.instance.ChangeVolume(AudioManager.instance.volumeSFX, AudioManager.instance.sfxObj);
        muteSFX.gameObject.SetActive(volumeSFX.value == 0 ? true : false);
        sfxValueTxt.text = ((AudioManager.instance.volumeSFX * 100).ToString("0.") + "%");
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
