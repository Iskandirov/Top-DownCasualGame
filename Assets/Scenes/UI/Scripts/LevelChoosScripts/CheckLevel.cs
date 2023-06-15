using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CheckLevel : MonoBehaviour
{
    [SerializeField]
    public List<SavedLocationsData> levels = new List<SavedLocationsData>();
    public List<SavedLocationsData> levelsRead = new List<SavedLocationsData>();
    private void Start()
    {
        SaveInventory();
        LoadInventory(levelsRead);
    }
    public void LoadInventory(List<SavedLocationsData> itemsRead)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(jsonLine);
                itemsRead.Add(data);
            }
        }
        else
        {
            File.Create(path);
        }
    }


    private void SaveInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        StreamWriter writer = new StreamWriter(path, true);

        SavedLocationsData data = new SavedLocationsData();
        foreach (SavedLocationsData item in levels)
        {
            data.IDLevel = item.IDLevel;
            data.percent = item.percent;


            string jsonData = JsonUtility.ToJson(data);
            writer.WriteLine(jsonData);
        }
        writer.Close();

    }
    public void SaveInventory(int level, int percent)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        string[] lines = File.ReadAllLines(path);
        bool objectFound = false;

        for (int i = 0; i < lines.Length; i++)
        {
            SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(lines[i]);
            if (data.IDLevel == level)
            {
                data.percent = percent;
                lines[i] = JsonUtility.ToJson(data);
                objectFound = true;

                break;
            }
        }

        if (!objectFound)
        {
            SavedLocationsData newData = new SavedLocationsData();
            newData.IDLevel = level;
            newData.percent = 0;
            List<string> updatedLines = lines.ToList();
            updatedLines.Add(JsonUtility.ToJson(newData));
            lines = updatedLines.ToArray();
        }

        File.WriteAllLines(path, lines);
    }
    public void CheckPercent(int level, int percentNew)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        string[] lines = File.ReadAllLines(path);
        for (int i = 0; i < lines.Length; i++)
        {
            SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(lines[i]);
            if (data.IDLevel == level && data.percent < percentNew)
            {
                data.percent = percentNew;
                lines[i] = JsonUtility.ToJson(data);
                break;
            }
        }
        File.WriteAllLines(path, lines);
    }
}
