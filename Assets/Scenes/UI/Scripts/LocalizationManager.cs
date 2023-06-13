using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
    public GameObject testText;
    public TMP_Dropdown drop;
    [SerializeField]
    [Header("TextLocalized")]
    public List<LocalizationDataItems> localizedText;
    [Header("Language"),Space(10)]
    public List<string> language = new List<string>();

    [SerializeField]
    private List<LocalizationDataItems> items = new List<LocalizationDataItems>();
    [Header("Settings")]
    public List<SettingsData> dataSett = new List<SettingsData>();
    private string loc = "eng";
    
    public bool IsSettingsPage;
    public float volume;
    void Awake()
    {
        //testText = GameObject.Find("Text (TMP)");
        StartLoad();
    }
    public void StartLoad()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Localization.txt");
        string filePathSett = Path.Combine(Application.persistentDataPath, "Settings.txt");
        if (File.Exists(filePath))
        {
            LoadLocalizedText();
        }
        else
        {
            SaveLocalization();
        }
        if (File.Exists(filePathSett))
        {
            LoadSettings();
        }
        else
        {
            SaveWithDefaultParams();
        }
    }
    public void LoadSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Settings.txt");

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string item in lines)
            {
                SettingsData data = JsonUtility.FromJson<SettingsData>(item);
                SettingsData settings = new SettingsData();
                settings.key = data.key;
                settings.value = data.value;
                dataSett.Add(settings);
                if (settings.key == "language" && IsSettingsPage == true)
                {
                    drop.value = language.IndexOf(settings.value);

                    List<GameObject> text = new List<GameObject>
                    {
                        testText
                    };
                    UpdateText(text);
                }
                if (settings.key == "volume" && IsSettingsPage == true)
                {
                    if (float.TryParse(settings.value,out float res))
                    {
                        volume = res;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }
    public void LoadLocalizedText()
    {
        localizedText = new List<LocalizationDataItems>();
        string filePath = Path.Combine(Application.persistentDataPath, "Localization.txt");

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string item in lines)
            {
                LocalizationDataItems data = JsonUtility.FromJson<LocalizationDataItems>(item);
                LocalizationDataItems dataItem = new LocalizationDataItems();
                dataItem.language = data.language;
                dataItem.key = data.key;
                dataItem.value = data.value;

                if (!language.Contains(dataItem.language))
                {
                    language.Add(dataItem.language);
                }
                localizedText.Add(dataItem);
            }

            //Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        }
    }
    public void SaveLocalization()
    {
        string path = Path.Combine(Application.persistentDataPath, "Localization.txt");
        StreamWriter writer = new StreamWriter(path, true);
        LocalizationDataItems data = new LocalizationDataItems();

        foreach (LocalizationDataItems item in items)
        {
            data.language = item.language;
            data.key = item.key;
            data.value = item.value;

            string jsonData = JsonUtility.ToJson(data);
            writer.WriteLine(jsonData);
        }
        writer.Close();
    }
    public void SaveWithDefaultParams()
    {
        List<SettingsData> list = new List<SettingsData>();

        string path = Path.Combine(Application.persistentDataPath, "Settings.txt");

        // ��������� �������� �� ������
        list.Add(new SettingsData { key = "language", value = "eng" });
        list.Add(new SettingsData { key = "volume", value = "0.5" });
        list.Add(new SettingsData { key = "v-sync", value = "0" });


        // ³������� ����� ��� ������
        StreamWriter writer = new StreamWriter(path, false);

        // ����� ��� �������� � ������ �� �����
        foreach (var item in list)
        {
            string jsonData = JsonUtility.ToJson(item);
            writer.WriteLine(jsonData);
        }

        // �������� �����
        writer.Close();

        // ���������������� ����������� �� ��������� ������
        LoadSettings();
        List<GameObject> text = new List<GameObject>
        {
            testText
        };
        UpdateText(text);
    }
    public void ChangeLanguage()
    {
        List<SettingsData> list = new List<SettingsData>();

        string path = Path.Combine(Application.persistentDataPath, "Settings.txt");

        if (File.Exists(path))
        {
            // ���������� ��� ����� � �����
            string[] lines = File.ReadAllLines(path);
            foreach (string item in lines)
            {
                SettingsData settings = JsonUtility.FromJson<SettingsData>(item);
                if (settings.key != "language")
                {
                    list.Add(settings);
                }
            }
        }

        // ��������� ������ �������� � ������ language �� ���� ���������
        SettingsData data = new SettingsData();
        data.key = "language";
        data.value = language[drop.value];
        // ��������� ������ �������� �� ������
        list.Add(data);

        // ��������, �� ��� ���� ������� � ������ language � ������
        bool hasLanguage = false;
        foreach (var item in list)
        {
            if (item.key == "language")
            {
                if (hasLanguage)
                {
                    // ���� ������� � ������ language ��� � � ������, �������� ����
                    list.Remove(item);
                }
                else
                {
                    // ���� ������� � ������ language � � ������, ���������� hasLanguage � true
                    hasLanguage = true;
                }
            }
        }

        // ³������� ����� ��� ������
        StreamWriter writer = new StreamWriter(path, false);

        // ����� ��� �������� � ������ �� �����
        foreach (var item in list)
        {
            string jsonData = JsonUtility.ToJson(item);
            writer.WriteLine(jsonData);
        }

        // �������� �����
        writer.Close();

        // ���������������� ����������� �� ��������� ������
        LoadSettings();
        List<GameObject> text = new List<GameObject>
        {
            testText
        };
        UpdateText(text);
    }
    public void ChangeVolume(float volume)
    {
        List<SettingsData> list = new List<SettingsData>();
        string path = Path.Combine(Application.persistentDataPath, "Settings.txt");

        if (File.Exists(path))
        {
            // ���������� ��� ����� � �����
            string[] lines = File.ReadAllLines(path);
            foreach (string item in lines)
            {
                SettingsData settings = JsonUtility.FromJson<SettingsData>(item);
                if (settings.key != "volume")
                {
                    list.Add(settings);
                }
            }
        }
        // ��������� ������ �������� � ������ language �� ���� ���������
        SettingsData data = new SettingsData();
        data.key = "volume";
        data.value = volume.ToString();

        // ��������� ������ �������� �� ������
        list.Add(data);

        // ��������, �� ��� ���� ������� � ������ language � ������
        bool hasLanguage = false;
        foreach (var item in list)
        {
            if (item.key == "volume")
            {
                if (hasLanguage)
                {
                    // ���� ������� � ������ language ��� � � ������, �������� ����
                    list.Remove(item);
                }
                else
                {
                    // ���� ������� � ������ language � � ������, ���������� hasLanguage � true
                    hasLanguage = true;
                }
            }
        }
        StreamWriter writer = new StreamWriter(path, false);
        // ����� ��� �������� � ������ �� �����
        foreach (var item in list)
        {
            string jsonData = JsonUtility.ToJson(item);
            writer.WriteLine(jsonData);
        }
        writer.Close();
    }
    public void ChangeLanguage(List<GameObject> text)
    {
        List<SettingsData> list = new List<SettingsData>();

        string path = Path.Combine(Application.persistentDataPath, "Settings.txt");

        if (File.Exists(path))
        {
            // ���������� ��� ����� � �����
            string[] lines = File.ReadAllLines(path);
            foreach (string item in lines)
            {
                SettingsData settings = JsonUtility.FromJson<SettingsData>(item);
                if (settings.key != "language")
                {
                    list.Add(settings);
                }
            }
        }

        // ��������� ������ �������� � ������ language �� ���� ���������
        SettingsData data = new SettingsData();
        data.key = "language";
        data.value = language[drop.value];

        // ��������� ������ �������� �� ������
        list.Add(data);

        // ��������, �� ��� ���� ������� � ������ language � ������
        bool hasLanguage = false;
        foreach (var item in list)
        {
            if (item.key == "language")
            {
                if (hasLanguage)
                {
                    // ���� ������� � ������ language ��� � � ������, �������� ����
                    list.Remove(item);
                }
                else
                {
                    // ���� ������� � ������ language � � ������, ���������� hasLanguage � true
                    hasLanguage = true;
                }
            }
        }

        // ³������� ����� ��� ������
        StreamWriter writer = new StreamWriter(path, false);

        // ����� ��� �������� � ������ �� �����
        foreach (var item in list)
        {
            string jsonData = JsonUtility.ToJson(item);
            writer.WriteLine(jsonData);
        }

        // �������� �����
        writer.Close();

        // ���������������� ����������� �� ��������� ������
        LoadSettings();

        text.Add(testText);
        UpdateText(text);
    }
    public void UpdateText(List<GameObject> text)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Localization.txt");

        if (File.Exists(filePath))
        {
            foreach (var lang in dataSett)
            {
                if (lang.key == "language")
                {
                    loc = lang.value;
                }
            }
            foreach (var item in localizedText)
            {
                foreach (var oneofmany in text)
                {
                    if (oneofmany.GetComponent<TagText>().tagText == item.key && loc == item.language)
                    {
                        oneofmany.GetComponent<TextMeshProUGUI>().text = item.value;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }
}

[System.Serializable]
public class LocalizationDataItems
{
    public string key;
    public string value;
    public string language;
}

[System.Serializable]
public class SettingsData
{
    public string key;
    public string value;
}
