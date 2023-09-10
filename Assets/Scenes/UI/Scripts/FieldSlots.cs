using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldSlots : MonoBehaviour
{
    public SetParametersToitem objToCraft;
    public Button childrenBtn;
    public LoadItemData data;
    char removeLevel;
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

        if (count > 3 && price <= coinT)
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
                //Видалення обєктів з яких крафтять
                string[] lines = File.ReadAllLines(path);
                List<string> updatedLines = new List<string>();
                int index = 0;
                foreach (string line in lines)
                {
                    string decrypt = hashing.Decrypt(line);
                    SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(decrypt);
                    if (index < 3)
                    {
                        if (data.Name + " " + data.Level != remove)
                        {
                            updatedLines.Add(line);
                        }
                        else
                        {
                            index++;
                            removeLevel = objToCraft.level[objToCraft.level.Length - 1];
                        }
                    }
                    else
                    {
                        updatedLines.Add(line);
                        remove = "";
                    }
                }
                // Записуємо оновлений масив рядків в файл
                string jsonContent = string.Join("\n", updatedLines.ToArray());
                string decryptedJson = hashing.Encrypt(jsonContent);
                File.WriteAllText(path, decryptedJson);
                //Створення нового обєкту
                // Створюємо новий об'єкт SavedObjectData
                foreach (string line in lines)
                {
                    SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(line);

                    if (data.Name + " " + data.Level == removeCopy && removeLevel.ToString() == data.Level.ToString())
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
                        // Конвертуємо об'єкт в рядок JSON
                        string newLine = JsonUtility.ToJson(newItem);
                        // Додаємо новий рядок до кінця файлу
                        string encryptedJson = hashing.Encrypt(newLine);
                        File.AppendAllText(path, encryptedJson + "\n");
                        break;
                    }
                }

                data.CleanList();
                data.LoadItems();
                objScore.score -= price;
                objScore.SaveScore((int)objScore.score, false);
                coinsTxt.text = objScore.score.ToString();
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
