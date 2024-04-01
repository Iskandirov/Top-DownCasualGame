using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PotionBase
{
    public PotionsType type;
    public int count;
    public string potionName;
    public string statName;
    public string statValue;
    public string description;
    public int reloadTime;
    public int price;
}
public class PotionSystem : MonoBehaviour
{
    public Image perkImage;
    public TextMeshProUGUI potionName;
    public TextMeshProUGUI statName;
    public TextMeshProUGUI statValue; 
    public TextMeshProUGUI reloadValue;
    public TextMeshProUGUI description;
    public TextMeshProUGUI price;
    public Button perkBuyButton;
    public TextMeshProUGUI money;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        List<TabButton> tabs = GetComponent<TabGroup>().tabButtons;
        foreach (var tab in tabs)
        {
            tab.GetComponent<Potion>().potion.potionName = tab.tooltipText;
        }
    }
    public void SetDescription()
    {
        Potion tab = GetComponent<TabGroup>().tabButtons.Find(t => t == GetComponent<TabGroup>().selectedTab).GetComponent<Potion>();
        perkImage.sprite = tab.perkImage.sprite;
        perkImage.SetNativeSize();
        potionName.text = tab.GetComponent<TabButton>().tooltipText;
        statName.text = tab.potion.statName;
        statValue.text = tab.potion.statValue;
        reloadValue.text = tab.potion.reloadTime.ToString();
        price.text = tab.potion.price.ToString();
        description.text = tab.potion.description;
        if (!PlayerPrefs.HasKey(tab.potion.type.ToString()))
        {
            PlayerPrefs.SetInt(tab.potion.type.ToString(), 0);
        }
        if (PlayerPrefs.GetInt(tab.potion.type.ToString()) > 0)
        {
            tab.potion.count = PlayerPrefs.GetInt(tab.potion.type.ToString());
        }
        perkBuyButton.interactable = int.Parse(price.text) <= int.Parse(money.text) ? true : false;
    }
    public void BuyPotion()
    {
        Potion tab = GetComponent<TabGroup>().tabButtons.Find(t => t == GetComponent<TabGroup>().selectedTab).GetComponent<Potion>();
        int potionCount = int.Parse(tab.count.text) + 1;
        tab.count.text = potionCount.ToString();
        money.text = GetScore.SaveMoney_Static(int.Parse(money.text) - int.Parse(tab.price.text));
        SetDescription();
        PlayerPrefs.SetInt(tab.potion.type.ToString(), potionCount);
    }
}
