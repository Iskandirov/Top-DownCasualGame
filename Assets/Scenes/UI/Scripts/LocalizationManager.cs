using Pathfinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

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
    public float vSync;
    public DataHashing hashing;
    void Awake()
    {
        StartLoad();
    }
    public void StartLoad()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Localization.txt");
        string filePathSett = System.IO.Path.Combine(Application.persistentDataPath, "Settings.txt");
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
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Settings.txt");
        if (File.Exists(filePath))
        {
            string[] encryptedText = File.ReadAllLines(filePath);

            foreach (string item in encryptedText)
            {
                // ������������ JSON �����
                string decrypt = hashing.Decrypt(item);
                SettingsData data = JsonUtility.FromJson<SettingsData>(decrypt);
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
                    if (float.TryParse(settings.value, out float res))
                    {
                        volume = res;
                    }
                }
                if (settings.key == "v-sync" && IsSettingsPage == true)
                {
                    if (float.TryParse(settings.value, out float res))
                    {
                        vSync = res;
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
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Localization.txt");
        if (File.Exists(filePath))
        {
            Task<string[]> task = File.ReadAllLinesAsync(filePath);

            // Wait for the task to complete
            task.Wait();

            // Get the result of the task
            string[] lines = task.Result;
            // ��������� ������������� ����� �� ����� �����
            foreach (string item in lines)
            {
                
                string decrypt = hashing.Decrypt(item);
                LocalizationDataItems data = JsonUtility.FromJson<LocalizationDataItems>(decrypt);
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
        }
    }
    public void SaveLocalization()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "Localization.txt");
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            foreach (LocalizationDataItems item in items)
            {
                LocalizationDataItems data = new LocalizationDataItems();
                data.language = item.language;
                data.key = item.key;
                data.value = item.value;

                string jsonData = JsonUtility.ToJson(data);
                string encryptedJson = hashing.Encrypt(jsonData);

                writer.WriteLine(encryptedJson); // ������� ��������� ";"
            }

            writer.Close();
        }

           
    }
    public void SaveWithDefaultParams()
    {
        List<SettingsData> list = new List<SettingsData>();
        string path = System.IO.Path.Combine(Application.persistentDataPath, "Settings.txt");
        // ��������� �������� �� ������
        list.Add(new SettingsData { key = "language", value = "eng" });
        list.Add(new SettingsData { key = "volume", value = "0.5" });
        list.Add(new SettingsData { key = "v-sync", value = "0" });
        // ³������� ����� ��� ������
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            // ����� ��� �������� � ������ �� �����
            foreach (var item in list)
            {
                string jsonData = JsonUtility.ToJson(item);
                string encryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(encryptedJson);
            }
            // �������� �����
            writer.Close();
        }
            
        // ���������������� ����������� �� ��������� ������
        LoadSettings();
        List<GameObject> text = new List<GameObject>
        {
            testText
        };
        UpdateText(text);
    }
    public void ChangeSetting(List<string> value)
    {
        List<SettingsData> list = new List<SettingsData>();
        string path = System.IO.Path.Combine(Application.persistentDataPath, "Settings.txt");
        if (File.Exists(path))
        {
            // ���������� ��� ����� � �����
            string[] lines = File.ReadAllLines(path);
            foreach (string item in lines)
            {
                string decryptedJson = hashing.Decrypt(item);
                SettingsData settings = JsonUtility.FromJson<SettingsData>(decryptedJson);
                if (settings.key != value[0])
                {
                    list.Add(settings);
                }
            }
        }
        // ��������� ������ �������� � ������ language �� ���� ���������
        SettingsData data = new SettingsData();
        data.key = value[0];
        data.value = value[1];
        // ��������� ������ �������� �� ������
        list.Add(data);
        // ��������, �� ��� ���� ������� � ������ language � ������
        bool hasLanguage = false;
        foreach (var item in list)
        {
            if (item.key == value[0])
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
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            // ����� ��� �������� � ������ �� �����
            foreach (var item in list)
            {
                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
            

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
        string path = System.IO.Path.Combine(Application.persistentDataPath, "Settings.txt");
        if (File.Exists(path))
        {
            // ���������� ��� ����� � �����
            string[] lines = File.ReadAllLines(path);
            foreach (string item in lines)
            {
                string decryptedJson = hashing.Decrypt(item);
                SettingsData settings = JsonUtility.FromJson<SettingsData>(decryptedJson);
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
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            // ����� ��� �������� � ������ �� �����
            foreach (var item in list)
            {
                string jsonData = JsonUtility.ToJson(item);
                string encryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(encryptedJson);
            }
            writer.Flush();
        }


        // ���������������� ����������� �� ��������� ������
        LoadSettings();
        List<GameObject> text = new List<GameObject>
        {
            testText
        };
        UpdateText(text);
    }
    public void UpdateText(List<GameObject> text)
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "Localization.txt");

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
