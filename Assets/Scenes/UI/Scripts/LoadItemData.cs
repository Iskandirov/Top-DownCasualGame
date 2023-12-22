using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class LoadItemData : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public List<SavedObjectData> objectsList;
    public List<GameObject> objectsListCopy;
    public List<Transform> parentsList;
    public FieldSlots craftObj;
    int index;
    public SetLanguage lang;
    public DataHashing hashing;
    void Start()
    {
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
                SavedObjectData itemParams = item;
                int count = objectCounts[(itemName, itemLevel)];

                GameObject newObject = Instantiate(prefabToInstantiate, transform);

                newObject.transform.SetParent(parentsList[index]); // index - це індекс батька у списку батьків
                newObject.transform.position = new Vector3(parentsList[index].position.x, parentsList[index].position.y + 1f, parentsList[index].position.z);
                newObject.transform.localScale = prefabToInstantiate.transform.localScale;

                SetParametersToitem objParam = newObject.GetComponent<SetParametersToitem>();
                objParam.ItemName = itemName;
                objParam.ItemImage.sprite = Resources.Load<Sprite>(itemParams.Name);
                objParam.ItemImage.SetNativeSize();
                objParam.Rare.sprite = Resources.Load<Sprite>(itemParams.RareName + " " + itemParams.Level.ToString());
                objParam.ItemStat.text = itemParams.Stat;
                objParam.level = itemParams.Level.ToString();
                objParam.RareName = itemParams.RareName;
                objParam.Count.text = count.ToString();
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

    public void CleanList()
    {
        for (int i = objectsList.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsListCopy[i].gameObject; // взяти GameObject елементу
            objectsList.RemoveAt(i);
            objectsListCopy.RemoveAt(i);
            Destroy(obj); // видалити елемент зі сцени
            index = 0;
        }
    }
}



