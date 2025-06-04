using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LoadItemData : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public List<SavedObjectData> objectsList;
    public List<GameObject> objectsListCopy;
    public List<Transform> parentItemsList;
    public FieldSlots craftObj;
    int index;
    public SetLanguage lang;
    public DataHashing hashing;


    public List<GameObject> equippedIndicators; // Індикатори одягнених предметів
    public List<GameObject> upgradeIndicators;  // Індикатори можли
    void Start()
    {
        foreach (var item in parentItemsList)
        {
            equippedIndicators.Add(item.GetComponent<Shelf>().checker);
            upgradeIndicators.Add(item.GetComponent<Shelf>().pointToUpgrade);
        }
        LoadItems();
    }
    public void LoadItems()
    {
        string fileName = Path.Combine(Application.persistentDataPath, "savedData.txt");
        if (File.Exists(fileName))
        {
            string[] jsonLines = File.ReadAllLines(fileName);

            Dictionary<(string, int), int> objectCounts = new Dictionary<(string, int), int>();
            Dictionary<(string, int), SavedObjectData> uniqueObjects = new Dictionary<(string, int), SavedObjectData>();

            foreach (string jsonLine in jsonLines)
            {
                string decrypt = hashing.Decrypt(jsonLine);
                SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(decrypt);

                (string, int) key = (data.Name, data.Level);
                if (uniqueObjects.ContainsKey(key))
                {
                    objectCounts[key] += 1;
                }
                else
                {
                    uniqueObjects.Add(key, data);
                    objectCounts.Add(key, 1);
                }
            }
            objectsList = new List<SavedObjectData>(uniqueObjects.Values);
            objectsList.Sort((x, y) => x.IDRare.CompareTo(y.IDRare));
            // Створення об'єктів з префаба для кожного унікального предмету та його кількості
            foreach (var item in objectsList)
            {
                string itemName = item.Name;
                int itemLevel = item.Level;
                item.Price = GameManager.Instance.itemsRead.Find(i => i.Name == itemName).Price;
                SavedObjectData itemParams = item;
                int count = objectCounts[(itemName, itemLevel)];

                GameObject newObject = Instantiate(prefabToInstantiate, transform);

                newObject.transform.SetParent(parentItemsList[index]);
                newObject.transform.position = new Vector3(parentItemsList[index].position.x, parentItemsList[index].position.y, parentItemsList[index].position.z);
                newObject.transform.localScale = prefabToInstantiate.transform.localScale;

                SetParametersToitem objParam = newObject.GetComponent<SetParametersToitem>();
                objParam.ItemName = itemName;
                objParam.ItemImage.sprite = GameManager.ExtractSpriteListFromTexture("items").First(i => i.name == itemParams.Name);
                objParam.ItemImage.SetNativeSize();
                objParam.ItemStat = itemParams.Stat;
                objParam.level = itemParams.Level.ToString();
                objParam.RareName = itemParams.RareName;
                objParam.Count = count.ToString();
                objParam.Price = itemParams.Price.ToString();
                objParam.Tag = itemParams.Tag;
                objParam.RareTag = itemParams.RareTag;
                objParam.IDRare = itemParams.IDRare;
                objParam.Description = itemParams.Description;
                newObject.GetComponent<MoveItem>().PointActivate();
                objectsListCopy.Add(newObject);
                lang.FindMyComponentInChildren(newObject, itemParams.Tag);
                index++; // переходимо до наступного батька у списку батьків
            }

        }
    }
    public void RemoveEquippedItemIfMatch(string itemName, int level)
    {
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (!File.Exists(path)) return;

        var lines = File.ReadAllLines(path);
        var updatedList = new List<SavedEquipData>();
        bool wasRemoved = false;

        foreach (string line in lines)
        {
            string decrypt = hashing.Decrypt(line);
            SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(decrypt);

            // Видаляємо лише якщо і Name, і Level співпадають
            if (!wasRemoved && data.Name == itemName && data.Level == level)
            {
                wasRemoved = true; // видаляємо тільки перший знайдений екземпляр
                continue;
            }
            updatedList.Add(data);
        }

        // Перезаписуємо файл без видаленого предмета
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (var equip in updatedList)
            {
                string json = JsonUtility.ToJson(equip);
                string encrypted = hashing.Encrypt(json);
                writer.WriteLine(encrypted);
            }
        }
    }
    private HashSet<string> GetEquippedKeys()
    {
        var keys = new HashSet<string>();
        for (int i = 0; i < objectsListCopy.Count; i++)
        {
            var itemUI = objectsListCopy[i].GetComponent<SetParametersToitem>();
            if (itemUI != null && equippedIndicators.Count > i && equippedIndicators[i] != null && equippedIndicators[i].activeSelf)
            {
                keys.Add(itemUI.ItemName + "_" + itemUI.level);
            }
        }
        return keys;
    }

    public void UpdateItemsUI(List<SavedObjectData> newData)
    {
        // Зберігаємо, які предмети були одягнені
        var equippedKeys = GetEquippedKeys();

        // Оновлення UI
        if (objectsListCopy.Count == newData.Count)
        {
            for (int i = 0; i < newData.Count; i++)
            {
                var itemUI = objectsListCopy[i].GetComponent<SetParametersToitem>();
                if (itemUI != null)
                {
                    itemUI.ItemName = newData[i].Name;
                    itemUI.level = newData[i].Level.ToString();
                    itemUI.Stat = newData[i].Stat;
                    // ... інші поля
                }
            }
        }
        else
        {
            CleanList();
            objectsList.Clear();
            objectsListCopy.Clear();
            objectsList.AddRange(newData);
            LoadItems();
        }

        // --- Очищаємо всі індикатори ---
        for (int i = 0; i < equippedIndicators.Count; i++)
        {
            if (equippedIndicators[i] != null)
                equippedIndicators[i].SetActive(false);
            if (upgradeIndicators[i] != null)
                upgradeIndicators[i].SetActive(false);
        }

        // --- Вмикаємо індикатори лише для тих предметів, які реально одягнені ---
        for (int i = 0; i < objectsListCopy.Count; i++)
        {
            var itemUI = objectsListCopy[i].GetComponent<SetParametersToitem>();
            if (itemUI == null) continue;
            string key = itemUI.ItemName + "_" + itemUI.level;

            if (equippedKeys.Contains(key) && equippedIndicators.Count > i && equippedIndicators[i] != null)
            {
                equippedIndicators[i].SetActive(true);
            }
            // Аналогічно для upgradeIndicators, якщо потрібно
        }
    }
    public void CleanList()
    {
        for (int i = objectsList.Count - 1; i >= 0; i--)
        {
            var itemUI = objectsListCopy[i].GetComponent<SetParametersToitem>();
            if (itemUI != null)
            {
                // Видаляємо з екіпірованих тільки якщо це саме цей предмет і рівень
                RemoveEquippedItemIfMatch(itemUI.ItemName, int.Parse(itemUI.level));
            }
            GameObject obj = objectsListCopy[i]; // взяти GameObject елементу
            objectsList.RemoveAt(i);
            objectsListCopy.RemoveAt(i);
            Destroy(obj); // видалити елемент зі сцени
            index = 0;
        }
    }
}



