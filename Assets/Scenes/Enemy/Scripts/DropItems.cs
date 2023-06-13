using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DropItems : MonoBehaviour
{
    public GameObject item1;
    public PoolItems items;
    public List<SavedObjectData> itemsLoaded;
    [Range(0.001f,1)]
    public float spawnRare = 0.6f;
    [Range(0.001f, 1)]
    public float spawnMiphical = 0.3f;
    [Range(0.001f, 1)]
    public float spawnLegendary = 0.05f;

    public List<SavedObjectData> CommonItems;
    public List<SavedObjectData> RareItems;
    public List<SavedObjectData> MiphicalItems;
    public List<SavedObjectData> LegendaryItems;

    public string[] rarityType = { "Звичайне", "Рідкісне", "Міфічне", "Легендарне" };
    
    GameObject bobs;
    public void Start()
    {
        bobs = GameObject.Find("Bobs_Parent");
    }
    public void OnDestroyBoss()
    {
        bobs.GetComponent<SpawnBobs>().isSpawned = false;
       
        ItemRarity();
        // Генеруємо випадкове число в діапазоні від 0 до 1
        float randomValue = Random.value;
        if (randomValue <= spawnLegendary)
        {
            SetStats(LegendaryItems);
        }
        else if (randomValue <= spawnMiphical)
        {
            SetStats(MiphicalItems);
        }
        else if (randomValue <= spawnRare)
        {
            SetStats(RareItems);
        }
        else if (randomValue <= 1)
        {
            SetStats(CommonItems);
        }
       
    }
    void SetStats(List<SavedObjectData> Rarity)
    {
        int rand = Random.Range(0, Rarity.Count);
        item1.GetComponent<ItemParameters>().itemName = Rarity[rand].Name;
        item1.GetComponent<ItemParameters>().itemImage = Rarity[rand].ImageSprite;
        item1.GetComponent<ItemParameters>().itemRareName = Rarity[rand].RareName;
        item1.GetComponent<ItemParameters>().itemRare = Rarity[rand].RareSprite;
        item1.GetComponent<ItemParameters>().idRare = Rarity[rand].IDRare;
        item1.GetComponent<ItemParameters>().Stat = Rarity[rand].Stat;
        item1.GetComponent<ItemParameters>().Level = Rarity[rand].Level;
        item1.GetComponent<ItemParameters>().Count = Rarity[rand].Count;
        item1.GetComponent<ItemParameters>().Tag = Rarity[rand].Tag;
        item1.GetComponent<ItemParameters>().RareTag = Rarity[rand].RareTag;
        Instantiate(item1, transform.position, transform.rotation);
    }
    void ItemRarity()
    {
        items.LoadInventory(itemsLoaded);
        foreach (SavedObjectData line in itemsLoaded)
        {
            if (line.RareName == rarityType[0])
            {
                CommonItems.Add(line);
            }
            else if (line.RareName == rarityType[1])
            {
                RareItems.Add(line);
            }
            else if (line.RareName == rarityType[2])
            {
                MiphicalItems.Add(line);
            }
            else if (line.RareName == rarityType[3])
            {
                LegendaryItems.Add(line);
            }

        }
    }
}
