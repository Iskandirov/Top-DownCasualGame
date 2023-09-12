using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using UnityEngine;

public class UpgradeObjInfo : MonoBehaviour
{
    [SerializeField]
    private List<SavedHouseholditemsData> items = new List<SavedHouseholditemsData>();
    public List<SavedHouseholditemsData> itemsRead = new List<SavedHouseholditemsData>();
    DataHashing hash;
    private void Awake()
    {
        hash = FindObjectOfType<DataHashing>();
        string path = Path.Combine(Application.persistentDataPath, "HouseHold.txt");
        if (!File.Exists(path))
        {
            SaveInventory();
        }
    
        LoadInventory(itemsRead);
    }
    public void LoadInventory(List<SavedHouseholditemsData> itemsRead)
    {
        string path = Path.Combine(Application.persistentDataPath, "HouseHold.txt");
        if (File.Exists(path))
        {

            string[] lines = File.ReadAllLines(path);

            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                string decrypt = hash.Decrypt(jsonLine);
                SavedHouseholditemsData data = JsonUtility.FromJson<SavedHouseholditemsData>(decrypt);

                data.UpgradeLevelImage.Add(Resources.Load<Sprite>(data.name + "_" + data.levelUpgrade));
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
        string path = Path.Combine(Application.persistentDataPath, "HouseHold.txt");
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            SavedHouseholditemsData data = new SavedHouseholditemsData();
            foreach (SavedHouseholditemsData item in items)
            {
                data.name = item.name;
                data.IDObject = item.IDObject;
                data.price = item.price;
                data.levelUpgrade = item.levelUpgrade;


                string jsonData = JsonUtility.ToJson(data);
                string decrypt = hash.Encrypt(jsonData);
                writer.WriteLine(decrypt);
            }
            writer.Close();
        }

            

    }
    public void SaveInventory(int level, int ID)
    {
        string path = Path.Combine(Application.persistentDataPath, "HouseHold.txt");
        string[] lines = File.ReadAllLines(path);
        string[] encryptedJson = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string decrypt = hash.Decrypt(lines[i]);
            SavedHouseholditemsData data = JsonUtility.FromJson<SavedHouseholditemsData>(decrypt);

            if (data.IDObject == ID)
            {
                data.levelUpgrade = level;
                lines[i] = JsonUtility.ToJson(data);
                encryptedJson[i] = hash.Encrypt(lines[i]); // Заповнюємо масив шифрованими даними
                break;
            }
        }
        File.WriteAllLines(path, encryptedJson);
    }
}
