using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class FieldSlots : MonoBehaviour
{
    public List<GameObject> objToCraft;
    public PopupMessage message;
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
    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<LoadItemData>();
        priceStart = price;
        childrenBtn = transform.parent.GetComponentInChildren<Button>();
        childrenBtn.interactable = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
    public void CheckCraft()
    {
        price = priceStart;
        if (objToCraft.Count == 3)
        {
            itemName = objToCraft[0].GetComponent<SetParametersToitem>().ItemName;
            char itemLevel = objToCraft[0].GetComponent<SetParametersToitem>().level[objToCraft[0].GetComponent<SetParametersToitem>().level.Length - 1];

            for (int i = 0; i < objToCraft.Count; i++)
            {
                removeLevel = objToCraft[i].GetComponent<SetParametersToitem>().level[objToCraft[i].GetComponent<SetParametersToitem>().level.Length - 1];
                if (objToCraft[i].GetComponent<SetParametersToitem>().ItemName != itemName || removeLevel != itemLevel
                    || objToCraft[i].GetComponent<SetParametersToitem>().level == "4")
                {
                    allItemsHaveSameName = false;
                    break;
                }
            }
            if (allItemsHaveSameName)
            {
                //Debug.Log("All items have the same name: " + itemName);
                if (int.TryParse(objToCraft[0].GetComponent<SetParametersToitem>().level, out int result))
                {
                    // Вдале перетворення
                    price = result * priceStart * objToCraft[0].GetComponent<SetParametersToitem>().IDRare;
                    priceTxt.text = price.ToString();
                    if (price <= FindObjectOfType<GetScore>().score)
                    {
                        childrenBtn.interactable = true;
                    }
                }
            }
            else
            {
                childrenBtn.interactable = false;
                //Debug.Log("Not all items have the same name");
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
        
        if (!objToCraft[0].GetComponent<SetParametersToitem>().level.Equals("4"))
        {
            string remove = objToCraft[0].GetComponent<SetParametersToitem>().ItemName + " " + objToCraft[0].GetComponent<SetParametersToitem>().level;
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
                            removeLevel = objToCraft[0].GetComponent<SetParametersToitem>().level[objToCraft[0].GetComponent<SetParametersToitem>().level.Length - 1];
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

                        int statCount = int.Parse(data.Stat);
                        statCount *= 2;
                        newItem.Stat = statCount.ToString();

                        newItem.Level = data.Level + 1;

                        newItem.Count = data.Count;



                        // Конвертуємо об'єкт в рядок JSON
                        string newLine = JsonUtility.ToJson(newItem);
                        // Додаємо новий рядок до кінця файлу
                        File.AppendAllText(path, newLine + "\n");
                        break;
                    }
                }

                data.CleanList();
                data.LoadItems();
                FindObjectOfType<GetScore>().score -= price;
                FindObjectOfType<GetScore>().SaveScore((int)FindObjectOfType<GetScore>().score);
                coinsTxt.text = FindObjectOfType<GetScore>().score.ToString();
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
