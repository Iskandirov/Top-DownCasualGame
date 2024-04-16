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
        loader.selectedSlots.Find(slot => slot.gameObject == EquipedBySlot).sprite = loader.baseSlotImage;
        loader.selectedSlots.Find(slot => slot.gameObject == EquipedBySlot).SetNativeSize();
        EquipedBySlot = null;
        basePotion.isActive = false;
        activeObj.SetActive(false);
        deselectButton.gameObject.SetActive(false);
        loader.selectedPotion = null;
        PlayerPrefs.SetString(basePotion.key + "Bool", basePotion.isActive.ToString());
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
                selected.deselectButton.gameObject.SetActive(false);
                selected.loader.selectedPotion = null;
                PlayerPrefs.SetString(selected.basePotion.key + "Bool", selected.basePotion.isActive.ToString());
            }
            EquipedBySlot = loader.selectedPotionSlot.gameObject;
            loader.selectedPotionSlot.sprite = potionImage.sprite;
            loader.selectedPotionSlot.SetNativeSize();
            loader.selectedPotion = this;
            basePotion.isActive = true;
            activeObj.SetActive(true);
            deselectButton.gameObject.SetActive(true);
            PlayerPrefs.SetString(basePotion.key + "Bool", basePotion.isActive.ToString());
        }
    }
}
