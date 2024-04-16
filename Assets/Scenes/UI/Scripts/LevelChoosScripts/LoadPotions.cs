using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadPotions : MonoBehaviour
{
    public List<Potions> potionsType;
    public PotionSelect potionPrefab;
    public PotionSelect selectedPotion;
    public Image selectedPotionSlot;
    public List<Image> selectedSlots;
    public Sprite baseSlotImage;
    public List<PotionSelect> loadedpotions;
    private void OnEnable()
    {
        SetPotionValue(potionsType, false);
        int offset = 120;
        RectTransform parentRect = GetComponent<RectTransform>();
        //if (transform.childCount <= 1)
        {
            foreach (var potion in potionsType)
            {
                //PlayerPrefs.DeleteKey(potion.key + "Bool");

                if (potion.value > 0 && loadedpotions.FirstOrDefault(l => l.basePotion.key == potion.key) == null)
                {
                    Debug.Log(1);
                    PotionSelect example = Instantiate(potionPrefab, transform.parent);
                    example.transform.SetParent(transform, false);
                    example.transform.position = Vector3.zero;
                    example.CountText.text = potion.value.ToString();
                    Sprite[] sprites = GameManager.ExtractSpriteListFromTexture("Quest");
                    example.potionImage.sprite = sprites.First(p => p.name == potion.key);
                    example.potionImage.SetNativeSize();
                    example.loader = this;
                    example.basePotion = potion;
                    potion.isActive = false;
                    loadedpotions.Add(example);
                }
            }
            foreach (var potion in loadedpotions)
            {
                potion.transform.localPosition = new Vector3(-parentRect.offsetMin.x + offset, parentRect.offsetMin.y - 100, 0);
                offset += 120;
            }
        }
    }
    public void SelectSlot()
    {
        selectedPotionSlot = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Image>();
    }
    public static void SetPotionValue(List<Potions> potionsType, bool isResset)
    {
        PotionsType[] potions = (PotionsType[])Enum.GetValues(typeof(PotionsType));
        foreach (PotionsType potion in potions)
        {
            Potions p = potionsType.Find(po => po.key == potionsType.Find(p => p.parameters == potion).key);
            if (PlayerPrefs.HasKey(p.key.ToString()))
            {
                p.value = PlayerPrefs.GetInt(p.key.ToString());
                if (isResset && p.isActive)
                {
                    PlayerPrefs.SetInt(p.key.ToString(), (int)p.value - 1);
                }
            }
        }
    }
}
