using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveitemToInventory : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EditorOnly"))
        {
            // Створення об'єкта даних
            SavedObjectData data = new SavedObjectData();
            data.Name = collision.GetComponent<ItemParameters>().itemName;
            data.IDRare = collision.GetComponent<ItemParameters>().idRare;
            data.RareName = collision.GetComponent<ItemParameters>().itemRareName;
            data.Stat = collision.GetComponent<ItemParameters>().Stat;
            data.Level = collision.GetComponent<ItemParameters>().Level;
            data.Tag = collision.GetComponent<ItemParameters>().Tag;
            data.RareTag = collision.GetComponent<ItemParameters>().RareTag;

            // Збереження даних у файл
            string fileName = Path.Combine(Application.persistentDataPath, "savedData.txt");
            StreamWriter writer = new StreamWriter(fileName, true);
            string jsonData = JsonUtility.ToJson(data);
            writer.WriteLine(jsonData);
            writer.Close();

            Destroy(collision.gameObject);
        }
    }
}




[System.Serializable]
public class SavedObjectData
{
    public string Name;
    public Sprite ImageSprite;
    public string RareName;
    public Sprite RareSprite;
    public int IDRare;
    public string Stat;
    public int Level;
    public int Count;
    public string Tag;
    public string RareTag;
}

[System.Serializable]
public class SavedUpgradeImage
{
    public int ID;
    public Sprite ImageSprite;
    public int IDRare;
}

[System.Serializable]
public class SavedEquipData
{
    public string Name;
    public Sprite ImageSprite;
    public string Stat;
    public int Level;
    public string Tag;
}
[System.Serializable]
public class SavedEconomyData
{
    public int money;
}
[System.Serializable]
public class SavedHouseholditemsData
{
    public string name;
    public int price;
    public int IDObject;
    public List<Sprite> UpgradeLevelImage;
    public int levelUpgrade;
}
[System.Serializable]
public class SavedLocationsData
{
    public int IDLevel;
    public int percent;
}
[System.Serializable]
public class SavedSkillsData
{
    public string Name;
    public int ID;
    public int level;
    public GameObject skillObj;
    public bool isPassive;
    public Sprite Image;
    public float CD;
    public List<string> tag;
    public string tagRare;
    public List<string> Description;
    public List<float> stat1;
    
}



