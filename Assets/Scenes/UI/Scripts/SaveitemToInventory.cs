using System.Collections.Generic;
using UnityEngine;

public class SaveitemToInventory : MonoBehaviour
{
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
    public string Description;
}
[System.Serializable]
public class SavedCharacterData
{
    public string Name;
    public int ID;
    public bool isBuy;
    public bool isEquiped;
    public string status;
    public string health;
    public string damage;
    public string move;
    public string attackSpeed;
    public string spell;
    public string spellCD;
    public bool interactable;
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
}
[System.Serializable]
public class SavedSkillsData
{
    public string Name;
    public int ID;
    public int level;
    public SkillBase skil;
    public bool isPassive;
    public Sprite Image;
    public List<string> tag;
    public string tagRare;
    public List<string> Description;
    public string type;
    public int MaxLevel { get; }
    public SavedSkillsData()
    {
        type = "base";
        level = 0;
        MaxLevel = 5;
    }
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





