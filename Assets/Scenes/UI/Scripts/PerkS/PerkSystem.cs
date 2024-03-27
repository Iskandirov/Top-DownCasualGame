using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PerkBase 
{
    public bool isActive;
    public bool buyed;
    public int level;
    public int row;
}

public class PerkSystem : MonoBehaviour
{
    [SerializeField] List<Perk> perks;

    [SerializeField] Color32 UnActive;
    [SerializeField] Color32 Active;
    [SerializeField] Sprite ActiveImg;
    static PerkSystem instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        ReloadPerkData();
    }
    public void ReloadPerkData()
    {
        foreach (var perk in perks)
        {
            if (!perk.perk.isActive && perks.Find(p => p.perk.level == perk.perk.level - 1 && (p.perk.row == perk.perk.row || p.perk.row == 0) && p.perk.level >= 1).perk.buyed)
            {
                perk.perk.isActive = true;
            }
            if (perk.perk.buyed)
            {
                perk.BuyedPerksImg.gameObject.SetActive(true);
                if (perks.Find(p => perk.perk.row == 0 && perk.perk.buyed))
                {
                    List<Perk> perkNextLevel = perks.FindAll(p => p.perk.level == perk.perk.level + 1);
                    foreach (var skill in perkNextLevel)
                    {
                        skill.perk.isActive = true;
                    }
                }
                else
                {
                    perks.Find(p => p.perk.level == perk.perk.level + 1 && perk.perk.level + 1 < 6 && p.perk.row == perk.perk.row).perk.isActive = true;
                }
            }
            if (!perk.perk.isActive && !perk.perk.buyed
                && !perks.Find(p => p.perk.level == perk.perk.level - 1 && (p.perk.row == perk.perk.row || p.perk.row == 0)).perk.buyed)
            {
                perk.GetComponent<Image>().color = UnActive;
                perk.GetComponentInChildren<Image>().color = UnActive;
            }
            else
            {
                perk.GetComponent<Image>().color = Active;
                perk.GetComponentInChildren<Image>().color = Active;
            }
        }
    }
    public static void ReloadPerkData_Static()
    {
        instance.ReloadPerkData();
    }
}
