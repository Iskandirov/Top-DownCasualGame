using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadPotions : MonoBehaviour
{
    public List<Potions> potionsType;
    public GameObject potionPrefab;
    public GameObject selectedPotion;
    private void OnEnable()
    {
        PotionsType[] potions = (PotionsType[])Enum.GetValues(typeof(PotionsType));

        foreach (PotionsType potion in potions)
        {
            Potions p = potionsType.Find(p => p.parameters == potion);
            if (PlayerPrefs.HasKey(potion.ToString()))
            {
                p.value = PlayerPrefs.GetInt(potion.ToString()) > 0 ? PlayerPrefs.GetInt(potion.ToString()) : 0;
            }
        }
        int offset = 120;
        RectTransform parentRect = GetComponent<RectTransform>();
        foreach (var potion in potionsType)
        {
            if (potion.value > 0)
            {
                GameObject example = Instantiate(potionPrefab, transform.parent);
                example.transform.SetParent(transform, false);
                example.transform.position = Vector3.zero;
                example.transform.localPosition = new Vector3(-parentRect.offsetMin.x + offset, parentRect.offsetMin.y - 100, 0);
                example.GetComponentInChildren<TextMeshProUGUI>().text = potion.value.ToString();
                example.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(potion.key);
                example.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                example.GetComponent<Button>().onClick.AddListener(OnClickHandler);
                offset += 120;
            }
        }
    }
    void OnClickHandler()
    {
        Debug.Log(1);
        selectedPotion = transform.parent.GetComponentsInChildren<Button>().First(b => b.).gameObject;
    }
}
