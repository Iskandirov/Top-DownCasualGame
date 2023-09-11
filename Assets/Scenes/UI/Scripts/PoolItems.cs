using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UIElements.Experimental;

public class PoolItems : MonoBehaviour
{
    [SerializeField]
    private List<SavedObjectData> items = new List<SavedObjectData>();
    [SerializeField]
    private List<SavedUpgradeImage> upgrades = new List<SavedUpgradeImage>();
    public List<SavedObjectData> itemsRead = new List<SavedObjectData>();

    public DataHashing hashing;
    private void Awake()
    {
        string path = Path.Combine(Application.persistentDataPath, "ItemInventory.txt");
        if (!File.Exists(path))
        {
            SaveInventory();
        }
        path = Path.Combine(Application.persistentDataPath, "UpgradeImage.txt");
        if (!File.Exists(path))
        {
            SaveUpgrade();
        }
        LoadInventory(itemsRead);
    }

    public void LoadInventory(List<SavedObjectData> itemsRead)
    {
        string path = Path.Combine(Application.persistentDataPath, "ItemInventory.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                // Розшифрувати JSON рядок
                string decryptedJson = hashing.Decrypt(jsonLine);

                SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(decryptedJson);

                data.ImageSprite = Resources.Load<Sprite>(data.Name);

                data.RareSprite = Resources.Load<Sprite>(data.RareName + " " + data.Level);
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
        string path = Path.Combine(Application.persistentDataPath, "ItemInventory.txt");
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            SavedObjectData data = new SavedObjectData();
            foreach (SavedObjectData item in items)
            {
                data.Name = item.Name;
                data.IDRare = item.IDRare;
                data.RareName = item.RareName;
                data.Stat = item.Stat;
                data.Level = item.Level;
                data.Count = item.Count;
                data.Tag = item.Tag;
                data.RareTag = item.RareTag;

                string jsonData = JsonUtility.ToJson(data);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
        }
    }
    private void SaveUpgrade()
    {
        string path = Path.Combine(Application.persistentDataPath, "UpgradeImage.txt");
        StreamWriter writer = new StreamWriter(path, true);

        SavedUpgradeImage data = new SavedUpgradeImage();
        foreach (SavedUpgradeImage item in upgrades)
        {
            data.ID = item.ID;
            data.IDRare = item.IDRare;

            string jsonData = JsonUtility.ToJson(data);
            string decryptedJson = hashing.Encrypt(jsonData);
            writer.WriteLine(decryptedJson);
        }
        writer.Close();
    }
}

