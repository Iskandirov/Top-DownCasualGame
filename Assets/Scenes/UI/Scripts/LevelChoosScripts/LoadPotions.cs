using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadPotions : MonoBehaviour
{
    public List<Potions> potionsType;
    public PotionSelect potionPrefab;
    public PotionSelect selectedPotion;
    public Image selectedPotionSlot;
    public Sprite baseSlotImage;
    private void OnEnable()
    {
        PotionsType[] potions = (PotionsType[])Enum.GetValues(typeof(PotionsType));

        foreach (PotionsType potion in potions)
        {
            Potions p = potionsType.Find(p => p.parameters == potion);
            if (PlayerPrefs.HasKey(potion.ToString()))
            {
                p.value = PlayerPrefs.GetInt(potion.ToString());
            }
        }
        int offset = 120;
        RectTransform parentRect = GetComponent<RectTransform>();
        if (transform.childCount <= 1)
        {
            foreach (var potion in potionsType)
            {
                if (potion.value > 0)
                {
                    PotionSelect example = Instantiate(potionPrefab, transform.parent);
                    example.transform.SetParent(transform, false);
                    example.transform.position = Vector3.zero;
                    example.transform.localPosition = new Vector3(-parentRect.offsetMin.x + offset, parentRect.offsetMin.y - 100, 0);
                    example.CountText.text = potion.value.ToString();
                    example.potionImage.sprite = Resources.Load<Sprite>(potion.key);
                    example.potionImage.SetNativeSize();
                    example.loader = this;
                    example.basePotion = potion;
                    offset += 120;
                }
            }
        }
    }
    public void SelectSlot()
    {
        selectedPotionSlot = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Image>();
    }
    
}
