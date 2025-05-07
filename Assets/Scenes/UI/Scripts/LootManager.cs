using System.Collections.Generic;
using UnityEngine;
using System;

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
    System.Random random = new System.Random();
    public int activeItemsCount = 0;
    public string[] rarityType = { "Звичайне", "Рідкісне", "Міфічне", "Легендарне" };
    private void Awake()
    {
        inst = this;
        if (itemPanel == null)
        {
            itemPanel = GameObject.Find("/LevelUpgrade").GetComponent<LootManager>().itemPanel;
        }
    }
    public void DropLoot(bool isTutor, Transform pos)
    {
        Debug.Log("DropLoot");
        ItemRarity();
        int itemCount = random.Next(3, 11); // Количество предметов от 3 до 10
        float angleStep = 360f / itemCount; // Угол между предметами

        for (int i = 0; i < itemCount; i++)
        {
            float angle = i * angleStep;
            Vector3 spawnPosition = pos.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * 20f; // Радиус круга 2 единицы
            float randomValue = (float)random.NextDouble();
            List<SavedObjectData> rarityItems = GetRarityItems(randomValue);
            if (rarityItems != null)
            {
                SetStats(rarityItems, isTutor, spawnPosition);
                activeItemsCount++;
            }
        }
    }
    public void ItemCollected()
    {
        activeItemsCount--; // Уменьшаем счетчик активных предметов
        //if (activeItemsCount <= 0)
        //{

        //    // Все предметы собраны, показываем панель завершения игры
        //    Timer dropLooted = FindObjectOfType<Timer>();
        //    dropLooted.isDropLooted = true;
        //}
    }

    void SetStats(List<SavedObjectData> Rarity, bool isTutor, Vector3 spawnPosition)
    {
        int rand = random.Next(0, Rarity.Count);

        ItemParameters newItem = Instantiate(itemRareObj[rand], spawnPosition, Quaternion.identity);
        newItem.itemName = Rarity[rand].Name;
        newItem.itemRareName = Rarity[rand].RareName;
        newItem.itemRare = Rarity[rand].RareSprite;
        newItem.idRare = Rarity[rand].IDRare;
        newItem.Stat = Rarity[rand].Stat;
        newItem.Level = Rarity[rand].Level;
        newItem.Price = Rarity[rand].Price;
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
