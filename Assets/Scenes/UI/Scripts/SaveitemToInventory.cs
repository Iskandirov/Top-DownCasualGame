using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveitemToInventory : MonoBehaviour
{
    [SerializeField]
    DataHashing hash;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EditorOnly"))
        {
            ItemParameters objParam = collision.GetComponent<ItemParameters>();
            // Створення об'єкта даних
            SavedObjectData data = new SavedObjectData();
            data.Name = objParam.itemName;
            data.IDRare = objParam.idRare;
            data.RareName = objParam.itemRareName;
            data.Stat = objParam.Stat;
            data.Level = objParam.Level;
            data.Tag = objParam.Tag;
            data.RareTag = objParam.RareTag;

            // Збереження даних у файл
            string fileName = Path.Combine(Application.persistentDataPath, "savedData.txt");
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                string jsonData = JsonUtility.ToJson(data);
                string decryptedJson = hash.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }

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
    public int countOfCount;
    public int countOfCountMax;
    public int percent;
    public bool isFullDone;
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
[System.Serializable]
public class SaveEnemyInfo
{
    public string Name;
    public Sprite Image;
    public float Health;
    public float Damage;
    public float MoveSpeed;
    public string Attack;
    public int ID;
    public bool Showed;
}





