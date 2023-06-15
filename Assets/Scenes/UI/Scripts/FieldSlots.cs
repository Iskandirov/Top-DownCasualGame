using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldSlots : MonoBehaviour
{
    public List<GameObject> objToCraft;
    public string messageObjectDifferent;
    public bool allItemsHaveSameName = true;
    public Button childrenBtn;
    string itemName;
    public LoadItemData data;
    char removeLevel;
    public int price;
    public int priceStart;
    public TextMeshProUGUI priceTxt;
    public TextMeshProUGUI coinsTxt;
    SetParametersToitem objParam;
    GetScore objScore;
    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<LoadItemData>();
        priceStart = price;
        childrenBtn = transform.parent.GetComponentInChildren<Button>();
        childrenBtn.interactable = false;
        objScore = FindObjectOfType<GetScore>();
    }
    public void CheckCraft()
    {
        objParam = objToCraft[0].GetComponent<SetParametersToitem>();

        price = priceStart;
        if (objToCraft.Count == 3)
        {
            itemName = objParam.ItemName;
            char itemLevel = objParam.level[objParam.level.Length - 1];
            for (int i = 0; i < objToCraft.Count; i++)
            {
                SetParametersToitem objParams = objToCraft[i].GetComponent<SetParametersToitem>();
                removeLevel = objParams.level[objParams.level.Length - 1];
                if (objParams.ItemName != itemName || removeLevel != itemLevel
                    || objParams.level == "4")
                {
                    allItemsHaveSameName = false;
                    break;
                }
            }
            if (allItemsHaveSameName)
            {
                if (int.TryParse(objParam.level, out int result))
                {
                    // Вдале перетворення
                    price = result * priceStart * objParam.IDRare;
                    priceTxt.text = price.ToString();
                    if (price <= objScore.score)
                    {
                        childrenBtn.interactable = true;
                    }
                }
            }
            else
            {
                childrenBtn.interactable = false;
            }
        }
        else if (objToCraft.Count < 3)
        {
            allItemsHaveSameName = true;
            childrenBtn.interactable = false;
        }
    }
    public void Craft()
    {
        if (!objParam.level.Equals("4"))
        {
            string remove = objParam.ItemName + " " + objParam.level;
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
                    SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(line);
                    if (index < 3)
                    {
                        if (data.Name + " " + data.Level != remove)
                        {
                            updatedLines.Add(line);
                        }
                        else
                        {
                            index++;
                            removeLevel = objParam.level[objParam.level.Length - 1];
                        }
                    }
                    else
                    {
                        updatedLines.Add(line);
                        remove = "";
                    }
                }
                // Записуємо оновлений масив рядків в файл
                File.WriteAllLines(path, updatedLines.ToArray());
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
                        File.AppendAllText(path, newLine + "\n");
                        break;
                    }
                }

                data.CleanList();
                data.LoadItems();
                objScore.score -= price;
                objScore.SaveScore((int)objScore.score);
                coinsTxt.text = objScore.score.ToString();
            }
        }
    }
    public void RemoveAllCreated()
    {
        for (int i = objToCraft.Count - 1; i >= 0; i--)
        {
            GameObject obj = objToCraft[i].gameObject; // взяти GameObject елементу
            objToCraft.RemoveAt(i);
            Destroy(obj); // видалити елемент зі сцени
        }
        childrenBtn.interactable = false;
    }

    public string CheckLevelUpgrade(SavedObjectData item)
    {
        string path = Path.Combine(Application.persistentDataPath, "UpgradeImage.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                SavedUpgradeImage data = JsonUtility.FromJson<SavedUpgradeImage>(line);
                if (data.IDRare == item.IDRare && data.ID == item.Level + 1)
                {
                    return data.ImageSprite + " " + data.ID.ToString();
                }
            }
        }
        return null;
    }
}
