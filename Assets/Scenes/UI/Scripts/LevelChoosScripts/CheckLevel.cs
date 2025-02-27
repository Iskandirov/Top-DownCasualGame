using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CheckLevel : MonoBehaviour
{
    [SerializeField]
    public List<SavedLocationsData> levels = new List<SavedLocationsData>();
    public List<SavedLocationsData> levelsRead = new List<SavedLocationsData>();
    public DataHashing hashing;
    private void Start()
    {
        hashing = FindObjectOfType<DataHashing>();
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        if (!File.Exists(path))
        {
            SaveInventory();
        }
        LoadInventory(levelsRead);
    }
    public void LoadInventory(List<SavedLocationsData> levelList)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                // Розшифрувати JSON рядок
                string decryptedJson = hashing.Decrypt(jsonLine);

                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(decryptedJson);

                levelList.Add(data);

            }
        }
    }


    private void SaveInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            foreach (SavedLocationsData item in levels)
            {
                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    public void SaveInventory(int level, int percent)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");

        List<SavedLocationsData> updatedData = new List<SavedLocationsData>();

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string decryptedJson = hashing.Decrypt(line);
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(decryptedJson);

                if (data.IDLevel == level)
                {
                    data.percent = percent;
                }

                updatedData.Add(data);
            }
        }

        // Add or update the data if not found
        bool found = false;
        foreach (SavedLocationsData data in updatedData)
        {
            if (data.IDLevel == level)
            {
                data.percent = percent;
                found = true;
                break;
            }
        }

        if (!found)
        {
            SavedLocationsData newData = new SavedLocationsData();
            newData.IDLevel = level;
            newData.percent = percent;
            updatedData.Add(newData);
        }

        // Write the updated data back to the file
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (SavedLocationsData data in updatedData)
            {
                string jsonData = JsonUtility.ToJson(data);
                string encryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(encryptedJson);
            }
            writer.Close();
        }
    }
    public void CheckPercent(int level, int percentNew)
    {
        hashing = FindObjectOfType<DataHashing>();
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            List<SavedLocationsData> updatedLines = new List<SavedLocationsData>();
            foreach (string line in lines)
            {
                string decrypt = hashing.Decrypt(line);
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(decrypt);
                if (data.IDLevel == level && data.percent <= percentNew)
                {
                    data.percent = percentNew;
                    if (percentNew >= 100)
                    {
                        data.percent = 0;
                        if (data.countOfCount < data.countOfCountMax)
                        {
                            data.countOfCount++;
                            if (data.countOfCount == data.countOfCountMax)
                            {
                                data.percent = 100;

                            }
                        }
                    }
                }
                updatedLines.Add(data);

            }
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                foreach (SavedLocationsData line in updatedLines)
                {
                    string jsonData = JsonUtility.ToJson(line);
                    string decryptedJson = hashing.Encrypt(jsonData);
                    writer.WriteLine(decryptedJson);
                }
                writer.Close();

            }
        }
    }
}
