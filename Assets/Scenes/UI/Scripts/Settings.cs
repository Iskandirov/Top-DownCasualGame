using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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
    List<Resolution> filtredResolutions;
    int currentResolutionIndex = 0;
    public Dictionary<int, int> resolutionVariations = new Dictionary<int, int>()
    {
        //{ 1024, 768 },   // Стандартне розширення для старих моніторів
        { 1280, 720 },   // HD (720p)
        { 1360, 768 },   // Поширене розширення для ноутбуків
        { 1366, 768 },   // Популярне розширення для ноутбуків
        { 1440, 900 },   // Поширене розширення для ноутбуків
        { 1600, 900 },   // HD+
        { 1680, 1050 },  // Поширене розширення для моніторів
        { 1920, 1080 },  // Full HD (1080p) - найпопулярніше розширення
        { 2048, 1152 },  // QWXGA
        { 2560, 1440 },  // Quad HD (1440p)
        { 3440, 1440 },  // UltraWide Quad HD
        //{ 3840, 2160 },  // Ultra HD (4K)
        //{ 5120, 2880 },  // 5K
        //{ 7680, 4320 },  // 8K
        //{ 10240, 4320 } // UltraWide 8K
    };
    private void Awake()
    {
        instance ??= this;
        SetResolutionStart();
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
            //SetVolumeMusic();
            //SetVolumeSFX();
            //SetVSync();
            //SetLanguage();
        }
    }
    public void SetResolutionStart()
    {
        filtredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();

        foreach (var screenSize in resolutionVariations)
        {
            Resolution newRes = new Resolution();
            newRes.width = screenSize.Key;
            newRes.height = screenSize.Value;
            newRes.refreshRateRatio = Screen.currentResolution.refreshRateRatio;
            filtredResolutions.Add(newRes);
            
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
        if (resolutionDropdown.IsActive())
        {
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        SetResolution(currentResolutionIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filtredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
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
        if (drop.IsActive())
        {
            drop.value = gameManager.language.IndexOf(gameManager.loc);
            toggle.isOn = Convert.ToBoolean(gameManager.vSync);
            volumeMusic.value = gameManager.volumeMusic;
            volumeSFX.value = gameManager.volumeSFX;
            AudioManager.instance.musicObj.volume = volumeMusic.value;
            AudioManager.instance.sfxObj.volume = volumeSFX.value;
            muteVolume.gameObject.SetActive(volumeMusic.value == 0 ? true : false);
            muteSFX.gameObject.SetActive(volumeSFX.value == 0 ? true : false);
        }
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
        Debug.Log("Slider value changed: " + value);
    }
    public void SetVolumeSFX()
    {

        List<string> valueSFX = new List<string>()
            {
            "sfx",
            volumeSFX.value.ToString(),
            };
        gameManager.ChangeSetting(valueSFX);
        AudioManager.instance.ChangeVolume(AudioManager.instance.volumeSFX, AudioManager.instance.sfxObj);
        muteSFX.gameObject.SetActive(volumeSFX.value == 0 ? true : false);
        sfxValueTxt.text = ((AudioManager.instance.volumeSFX * 100).ToString("0.") + "%");
        Debug.Log("Slider value changed: " + valueSFX);

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
        
        string languageRes = drop.options[drop.value].text == "Ukrainian" ? "ua" : "eng";
        List<string> value = new List<string>()
            {
            "language",
            languageRes,
            // Додаткові значення списку
            };
        gameManager.ChangeSetting(value);
    }
}
