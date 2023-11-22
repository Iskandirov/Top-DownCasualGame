using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    public ItemParameters itemPrefab;
    public PoolItems items;
    public List<SavedObjectData> itemsLoaded;
    public float spawnRare = 0.6f;
    public float spawnMiphical = 0.3f;
    public float spawnLegendary = 0.05f;

    public List<SavedObjectData> CommonItems;
    public List<SavedObjectData> RareItems;
    public List<SavedObjectData> MiphicalItems;
    public List<SavedObjectData> LegendaryItems;
    public bool isTutor;
    public string[] rarityType = { "��������", "г�����", "̳�����", "����������" };

    public void OnDestroyBoss()
    {
        ItemRarity();
        float randomValue = Random.value;
        List<SavedObjectData> rarityItems = GetRarityItems(randomValue);
        if (rarityItems != null && !isTutor)
            SetStats(rarityItems, isTutor);
        else if(rarityItems != null && isTutor)
            SetStats(rarityItems, isTutor);
    }

    void SetStats(List<SavedObjectData> Rarity,bool isTutor)
    {
        int rand = Random.Range(0, Rarity.Count);
        ItemParameters newItem = Instantiate(itemPrefab, transform.position, transform.rotation);
        newItem.itemName = Rarity[rand].Name;
        newItem.itemImage = Rarity[rand].ImageSprite;
        newItem.itemRareName = Rarity[rand].RareName;
        newItem.itemRare = Rarity[rand].RareSprite;
        newItem.idRare = Rarity[rand].IDRare;
        newItem.Stat = Rarity[rand].Stat;
        newItem.Level = Rarity[rand].Level;
        newItem.Count = Rarity[rand].Count;
        newItem.Tag = Rarity[rand].Tag;
        newItem.RareTag = Rarity[rand].RareTag;

        newItem.isTutor = isTutor;
    }

    List<SavedObjectData> GetRarityItems(float randomValue)
    {
        if (randomValue <= spawnLegendary)
            return LegendaryItems;
        else if (randomValue <= spawnMiphical)
            return MiphicalItems;
        else if (randomValue <= spawnRare)
            return RareItems;
        else if (randomValue <= 1)
            return CommonItems;

        return null;
    }

    void ItemRarity()
    {
        items.LoadInventory(itemsLoaded);
        foreach (SavedObjectData line in itemsLoaded)
        {
            switch (line.RareName)
            {
                case "��������":
                    CommonItems.Add(line);
                    break;
                case "г�����":
                    RareItems.Add(line);
                    break;
                case "̳�����":
                    MiphicalItems.Add(line);
                    break;
                case "����������":
                    LegendaryItems.Add(line);
                    break;
            }
        }
    }
}
