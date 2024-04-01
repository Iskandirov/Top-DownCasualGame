using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public TabButton selectedTab;
    [SerializeField] List<GameObject> objToSwap;
    [SerializeField] GameObject objToShow;
    [SerializeField] Tooltip tooltip;
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
            PerkSystem perkSytstem = GetComponent<PerkSystem>();
            PotionSystem potionSytstem = GetComponent<PotionSystem>();
            objToShow.SetActive(true);
            if (perkSytstem != null)
            {
                perkSytstem.SetDescription();
            }
            else
            {
                potionSytstem.SetDescription();
            }
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
        PerkSystem perkStstem = GetComponent<PerkSystem>();
        perkStstem.money.text = GetScore.SaveMoney_Static(int.Parse(perkStstem.money.text) - int.Parse(perkStstem.price.text));
        Perk perk = tabButtons.Find(b => b.tooltipText == perkStstem.perkName.text).GetComponent<Perk>();
        perk.perk.buyed = true;
        perkStstem.perkBuyButton.interactable = false;
        PerkSystem.ReloadPerkData_Static();
        string statValue = perk.statValue;
        string numericValue = "";

        foreach (char c in statValue)
        {
            if (char.IsDigit(c))
            {
                numericValue += c;
            }
        }
            PlayerPrefs.SetFloat(perk.statName, float.Parse(numericValue));
    }
}
