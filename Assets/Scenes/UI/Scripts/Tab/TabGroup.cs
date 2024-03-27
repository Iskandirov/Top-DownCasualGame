using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [SerializeField] List<TabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    [SerializeField] TabButton selectedTab;
    [SerializeField] List<GameObject> objToSwap;
    [SerializeField] GameObject objToShow;
    [SerializeField] Tooltip tooltip;

    [Header("BuyPerkSettings")]
    [SerializeField] Image perkImage;
    [SerializeField] TextMeshProUGUI perkName;
    [SerializeField] TextMeshProUGUI statName;
    [SerializeField] TextMeshProUGUI statValue;
    [SerializeField] TextMeshProUGUI price;
    [SerializeField] Button perkBuyButton;
    public TextMeshProUGUI money;
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }
    public void OnTabEnter(TabButton button,string text)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.bg.sprite = tabHover;
        }
        Tooltip.ShowTooltip_Static(text);
    }
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
        Tooltip.HideTooltip_Static();
        
    }
    public void OnTabSelected(TabButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        button.bg.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        if (objToSwap.Count != 0)
        {
            for (int i = 0; i < objToSwap.Count; i++)
            {
                if (i == index)
                {
                    objToSwap[i].SetActive(true);
                }
                else
                {
                    objToSwap[i].SetActive(false);
                }
            }
        }
        else
        {
            objToShow.SetActive(true);
            TabButton tab = tabButtons.Find(t => t == selectedTab);
            perkImage.sprite = tab.GetComponent<Perk>().perkImage.sprite;
            perkImage.SetNativeSize();
            perkName.text = tab.tooltipText;
            statName.text = tab.statName;
            statValue.text = tab.statValue;
            price.text = tab.price;
            perkBuyButton.interactable = int.Parse(price.text) <= int.Parse(money.text) 
                && tab.GetComponent<Perk>().perk.isActive && !tab.GetComponent<Perk>().perk.buyed ? true : false;

        }
    }
    void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.bg.sprite = tabIdle;
        }
    }
    public void GetResultScore()
    {
        money.text = GetScore.SaveMoney_Static(int.Parse(money.text) - int.Parse(price.text));
        Perk perk = tabButtons.Find(b => b.tooltipText == perkName.text).GetComponent<Perk>();
        perk.perk.buyed = true;
        perkBuyButton.interactable = false;
        PerkSystem.ReloadPerkData_Static();
    }
}
