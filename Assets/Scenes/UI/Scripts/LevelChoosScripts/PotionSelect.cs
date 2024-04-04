using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionSelect : MonoBehaviour
{
    public Potions basePotion;
    public GameObject EquipedBySlot;
    public TextMeshProUGUI CountText;
    public Image potionImage;
    public GameObject activeObj;
    public Button deselectButton;
    public LoadPotions loader;
    public void DeselectSlot()
    {

        
        loader.selectedPotionSlot.sprite = potionImage.sprite;
        loader.selectedPotionSlot.SetNativeSize();
        EquipedBySlot = null;
        basePotion.isActive = false;
        activeObj.SetActive(false);
        deselectButton.gameObject.SetActive(false);
        loader.selectedPotion = null;
    }
    public void SelectSlot()
    {
        PotionSelect selected = transform.parent.GetComponentsInChildren<PotionSelect>().FirstOrDefault(p => p.basePotion.isActive 
        && p.EquipedBySlot == loader.selectedPotionSlot.gameObject);
       

        if (loader.selectedPotion == null || loader.selectedPotion.gameObject != gameObject && basePotion.isActive == false)
        {
            if (selected != null)
            {
                selected.loader.selectedPotionSlot.sprite = potionImage.sprite;
                selected.loader.selectedPotionSlot.SetNativeSize();

                selected.activeObj.SetActive(false);
                selected.EquipedBySlot = null;
                selected.basePotion.isActive = false;
                selected.activeObj.SetActive(false);
                selected.deselectButton.gameObject.SetActive(false);
                selected.loader.selectedPotion = null;
            }
            EquipedBySlot = loader.selectedPotionSlot.gameObject;
            basePotion.isActive = true;
            activeObj.SetActive(true);
            deselectButton.gameObject.SetActive(true);
            EquipedBySlot = loader.selectedPotionSlot.gameObject;
            loader.selectedPotionSlot.sprite = potionImage.sprite;
            loader.selectedPotionSlot.SetNativeSize();
            loader.selectedPotion = this;

        }
    }
}
