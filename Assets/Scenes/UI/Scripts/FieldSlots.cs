using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        priceStart = price;
        childrenBtn.interactable = false;
        objScore = FindObjectOfType<GetScore>();
    }
    public void CheckCraft()
    {
        int count = int.Parse(objToCraft.Count);
        int level = int.Parse(objToCraft.level);
        int price = int.Parse(objToCraft.Price) * level;
        int coins = int.Parse(coinsTxt.text);

        priceTxt.text = price.ToString();
        childrenBtn.interactable = count >= 3 && coins >= price;
    }
    public void Craft()
    {
        if (int.Parse(objToCraft.level) >= 4) return;

        string path = Path.Combine(Application.persistentDataPath, "savedData.txt");
        if (!File.Exists(path)) return;

        string targetKey = $"{objToCraft.ItemName} {objToCraft.level}";
        List<SavedObjectData> originalData = File.ReadAllLines(path)
            .Select(line => JsonUtility.FromJson<SavedObjectData>(hashing.Decrypt(line)))
            .ToList();

        List<SavedObjectData> upgradedList = new();
        int consumedCount = 0;
        bool upgraded = false;

        foreach (var data in originalData)
        {
            string currentKey = $"{data.Name} {data.Level}";

            if (!upgraded && currentKey == targetKey && consumedCount < 3)
            {
                consumedCount++;
                if (consumedCount == 3)
                {
                    // Створюємо новий предмет з +1 рівнем
                    SavedObjectData upgradedItem = new()
                    {
                        Name = data.Name,
                        ImageSprite = data.ImageSprite,
                        RareName = data.RareName,
                        IDRare = data.IDRare,
                        Tag = data.Tag,
                        RareTag = data.RareTag,
                        Level = data.Level + 1,
                        Description = data.Description,
                        Price = data.Price,
                        Stat = (float.Parse(data.Stat) * 2).ToString(),
                        RareSprite = Resources.Load<Sprite>(CheckLevelUpgrade(data))
                    };
                    upgradedList.Add(upgradedItem);
                    upgraded = true;
                }
                continue; // Пропускаємо використані
            }

            upgradedList.Add(data);
        }

        if (!upgraded) return;

        // Перезапис файлу
        File.WriteAllLines(path, upgradedList.Select(item =>
        {
            string json = JsonUtility.ToJson(item);
            return hashing.Encrypt(json);
        }));

        EquipedStillHere(); // синхронізація екіпірованих
        data.UpdateItemsUI(upgradedList);

        // Знімаємо монети
        objScore.score -= price;
        objScore.SaveScore((int)objScore.score);
        coinsTxt.text = objScore.score.ToString();
        GameManager.Instance.ClosePanel(GameManager.Instance.menuPanel);
    }
   
    //public
    public void EquipedStillHere()
    {
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        string fileName = Path.Combine(Application.persistentDataPath, "savedData.txt");
        if (File.Exists(path))
        {
            if (File.Exists(fileName))
            {
                List<SavedEquipData> count = new List<SavedEquipData>();
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
                        if (data.Name == dataObj.Name && data.Level == dataObj.Level && count.FirstOrDefault(s => s.Name == data.Name) == null)
                        {
                            count.Add(data);
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
