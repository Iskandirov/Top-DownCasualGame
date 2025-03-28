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
            // ��������� ��'���� � ������� ��� ������� ����������� �������� �� ���� �������
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
                index++; // ���������� �� ���������� ������ � ������ ������
            }

        }
    }

    public void CleanList()
    {
        for (int i = objectsList.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsListCopy[i]; // ����� GameObject ��������
            objectsList.RemoveAt(i);
            objectsListCopy.RemoveAt(i);
            Destroy(obj); // �������� ������� � �����
            index = 0;
        }
    }
}



