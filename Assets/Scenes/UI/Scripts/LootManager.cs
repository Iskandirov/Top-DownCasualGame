using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager inst;
    public ItemParameters itemPanel;

    public List<ItemParameters> itemRareObj;
    public List<SavedObjectData> itemsLoaded;
    [Range(0,1)]
    public float spawnRare = 0.6f;
    [Range(0, 1)]
    public float spawnMiphical = 0.3f;
    [Range(0, 1)]
    public float spawnLegendary = 0.05f;

    public List<SavedObjectData> CommonItems;
    public List<SavedObjectData> RareItems;
    public List<SavedObjectData> MiphicalItems;
    public List<SavedObjectData> LegendaryItems;
    public bool isTutor;
    public string[] rarityType = { "Звичайне", "Рідкісне", "Міфічне", "Легендарне" };
    private void Awake()
    {
        inst = this;
    }
    public void DropLoot(bool isTutor,Transform pos)
    {
        ItemRarity();
        float randomValue = Random.value;
        List<SavedObjectData> rarityItems = GetRarityItems(randomValue);
        if (rarityItems != null)
            SetStats(rarityItems, isTutor, pos);
    }
    void SetStats(List<SavedObjectData> Rarity, bool isTutor, Transform objTransform)
    {
        int rand = Random.Range(0, Rarity.Count);
        ItemParameters newItem = Instantiate(itemRareObj[rand], objTransform.position, objTransform.rotation);
        newItem.itemName = Rarity[rand].Name;
        //newItem.itemImage = Rarity[rand].ImageSprite;
        newItem.itemRareName = Rarity[rand].RareName;
        newItem.itemRare = Rarity[rand].RareSprite;
        newItem.idRare = Rarity[rand].IDRare;
        newItem.Stat = Rarity[rand].Stat;
        newItem.Level = Rarity[rand].Level;
        newItem.Count = Rarity[rand].Count;
        newItem.Tag = Rarity[rand].Tag;
        newItem.RareTag = Rarity[rand].RareTag;
        newItem.Description = Rarity[rand].Description;

        newItem.isTutor = isTutor;
        newItem.itemPanel = itemPanel;
    }
    public static void SetPanelsInfoToLoot(ItemParameters newItem, ItemParameters itemPanel)
    {
        newItem.itemImage = itemPanel.itemImage;
        newItem.itemNameTxt = itemPanel.itemNameTxt;
        newItem.itemDescription = itemPanel.itemDescription;
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
        GameManager.Instance.LoadInventory(itemsLoaded);
        foreach (SavedObjectData line in itemsLoaded)
        {
            switch (line.RareName)
            {
                case "Звичайне":
                    CommonItems.Add(line);
                    break;
                case "Рідкісне":
                    RareItems.Add(line);
                    break;
                case "Міфічне":
                    MiphicalItems.Add(line);
                    break;
                case "Легендарне":
                    LegendaryItems.Add(line);
                    break;
            }
        }
    }
}
