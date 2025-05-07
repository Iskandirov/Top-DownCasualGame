using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButtons> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public TabButtons selectedTab;
    [SerializeField] List<GameObject> objToSwap;
    [SerializeField] GameObject objToShow;
    [SerializeField] Tooltip tooltip;
    [SerializeField] TabGroup tabs;
    public void Subscribe(TabButtons button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButtons>();
        }
        //tabButtons.Add(button);
    }
    public void OnTabEnter(TabButtons button,string text)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.bg.sprite = tabHover;
        }
        Tooltip.ShowTooltip_Static(text);
    }
    public void OnTabExit(TabButtons button)
    {
        ResetTabs();
        Tooltip.HideTooltip_Static();
        
    }
    public void OnTabSelected(TabButtons button)
    {
        int y = UnityEngine.Random.Range(1, 4);
        AudioManager.instance.PlaySFX("OpenPanel_" + y);
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
            CharacterSystem characterSytstem = GetComponent<CharacterSystem>();
            objToShow.SetActive(true);
            objToShow.transform.position = button.transform.position;
            if (perkSytstem != null)
            {
                perkSytstem.SetDescription();
            }
            else if(potionSytstem != null)
            {
                potionSytstem.SetDescription();
            }
            else if(characterSytstem != null)
            {
                characterSytstem.SetDescription();
            }
            else
            {
                tabs.objToSwap.Find(o => o.gameObject.activeSelf).SetActive(false);
            }
        }
    }
    void ResetTabs()
    {
        foreach (TabButtons button in tabButtons)
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
