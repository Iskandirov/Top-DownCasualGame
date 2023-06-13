using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LoadItemData : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public List<SavedObjectData> objectsList;
    public List<GameObject> objectsListCopy;
    public List<Transform> parentsList;
    public FieldSlots craftObj;
    int index;
    public SetLanguage lang;

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
                SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(jsonLine);
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
                newObject.GetComponent<SetParametersToitem>().ItemName = itemName;
                newObject.GetComponent<SetParametersToitem>().ItemImage.sprite = Resources.Load<Sprite>(itemParams.Name);
                newObject.GetComponent<SetParametersToitem>().ItemImage.SetNativeSize();
                newObject.GetComponent<SetParametersToitem>().Rare.sprite = Resources.Load<Sprite>(itemParams.RareName + " " + itemParams.Level.ToString());
                newObject.GetComponent<SetParametersToitem>().ItemStat.text = itemParams.Stat;
                newObject.GetComponent<SetParametersToitem>().level = itemParams.Level.ToString();
                newObject.GetComponent<SetParametersToitem>().RareName = itemParams.RareName;
                newObject.GetComponent<SetParametersToitem>().Count.text = count.ToString();
                newObject.GetComponent<SetParametersToitem>().Tag = itemParams.Tag;
                newObject.GetComponent<SetParametersToitem>().RareTag = itemParams.RareTag;
                newObject.GetComponent<SetParametersToitem>().IDRare = itemParams.IDRare;
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
            craftObj.RemoveAllCreated();
            Destroy(obj); // видалити елемент зі сцени
            index = 0;
        }
    }
}



