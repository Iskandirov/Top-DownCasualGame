using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldSlots : MonoBehaviour
{
    public SetParametersToitem objToCraft;
    public Button childrenBtn;
    public LoadItemData data;
    public int price;
    public int priceStart;
    public TextMeshProUGUI priceTxt;
    public TextMeshProUGUI coinsTxt;
    GetScore objScore;
    DataHashing hashing;
    // Start is called before the first frame update
    void Start()
    {
        hashing = FindObjectOfType<DataHashing>();
        data = FindObjectOfType<LoadItemData>();
        priceStart = price;
        childrenBtn.interactable = false;
        objScore = FindObjectOfType<GetScore>();
    }
    public void CheckCraft()
    {
        int count = int.Parse(objToCraft.GetComponent<SetParametersToitem>().Count.text);

        int level = int.Parse(objToCraft.GetComponent<SetParametersToitem>().level);

        price = priceStart * level;
        priceTxt.text = price.ToString();

        int coinT = int.Parse(coinsTxt.text);

        if (count >= 3 && price <= coinT)
        {
            childrenBtn.interactable = true;
        }
        else 
        {
            childrenBtn.interactable = false;
        }
    }
    public void Craft()
    {
        if (!objToCraft.level.Equals("4"))
        {
            string remove = objToCraft.ItemName + " " + objToCraft.level;
            string removeCopy = remove;
            string path = Path.Combine(Application.persistentDataPath, "savedData.txt");
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                List<SavedObjectData> updatedLines = new List<SavedObjectData>();
                // Створюємо новий об'єкт SavedObjectData
                foreach (string line in lines)
                {
                    string decrypt = hashing.Decrypt(line);
                    SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(decrypt);

                    if (data.Name + " " + data.Level == removeCopy && objToCraft.level[objToCraft.level.Length - 1].ToString() == data.Level.ToString())
                    {
                        SavedObjectData newItem = new SavedObjectData();
                        newItem.Name = data.Name;
                        newItem.ImageSprite = data.ImageSprite;
                        newItem.RareName = data.RareName;
                        newItem.RareSprite = Resources.Load<Sprite>(CheckLevelUpgrade(data));
                        newItem.IDRare = data.IDRare;
                        newItem.Level = data.Level + 1;
                        newItem.Count = data.Count;
                        int statCount = int.Parse(data.Stat);
                        statCount *= 2;
                        newItem.Stat = statCount.ToString();

                        updatedLines.Add(newItem);
                        //using (StreamWriter writer = new StreamWriter(path, true))
                        //{
                        //    string jsonData = JsonUtility.ToJson(newItem);
                        //    // Шифруємо дані перед записом у файл
                        //    string encryptedJson = hashing.Encrypt(jsonData);
                        //    writer.Write(encryptedJson);
                        //    writer.Close();
                        //}
                        break;
                    }
                }
                //Видалення обєктів з яких крафтять
                int index = 0;
                foreach (string line in lines)
                {
                    string decrypt = hashing.Decrypt(line);
                    SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(decrypt);
                    if (index < 3)
                    {
                        if (data.Name + " " + data.Level != remove)
                        {
                            updatedLines.Add(data);
                        }
                        else
                        {
                            index++;
                        }
                    }
                    else
                    {
                        updatedLines.Add(data);
                    }
                }
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    foreach (SavedObjectData line in updatedLines)
                    {
                        string jsonData = JsonUtility.ToJson(line);
                        string decryptedJson = hashing.Encrypt(jsonData);
                        writer.WriteLine(decryptedJson);
                    }
                    writer.Close();

                }

                EquipedStillHere();
                data.CleanList();
                data.LoadItems();
                objScore.score -= price;
                objScore.SaveScore((int)objScore.score, false);
                coinsTxt.text = objScore.score.ToString();
            }
        }
    }
    public void EquipedStillHere()
    {
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        string fileName = Path.Combine(Application.persistentDataPath, "savedData.txt");
        if (File.Exists(path))
        {
            if (File.Exists(fileName))
            {
                List<string> count = new List<string>();
                string[] jsonLines = File.ReadAllLines(fileName);
                string[] jsonData = File.ReadAllLines(path);
                foreach (var dataLine in jsonLines)
                {
                    string decryptData = hashing.Decrypt(dataLine);
                    SavedObjectData dataObj = JsonUtility.FromJson<SavedObjectData>(decryptData);
                    foreach (string jsonLine in jsonData)
                    {
                        string decrypt = hashing.Decrypt(jsonLine);
                        SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(decrypt);
                        if (data.Name == dataObj.Name && data.Level == dataObj.Level)
                        {
                            count.Add(jsonLine);
                        }
                    }
                }
                FindObjectOfType<EquipItem>().SaveUpdateEquip(path, count);
            }
        }
    }
    public string CheckLevelUpgrade(SavedObjectData item)
    {
        string path = Path.Combine(Application.persistentDataPath, "UpgradeImage.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                string decrypt = hashing.Decrypt(line);
                SavedUpgradeImage data = JsonUtility.FromJson<SavedUpgradeImage>(decrypt);
                if (data.IDRare == item.IDRare && data.ID == item.Level + 1)
                {
                    return data.ImageSprite + " " + data.ID.ToString();
                }
            }
        }
        return null;
    }
}
