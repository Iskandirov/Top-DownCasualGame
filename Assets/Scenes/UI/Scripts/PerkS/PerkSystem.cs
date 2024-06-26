using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] Color32 UnActivePerk;
    [SerializeField] Color32 ActivePerk;
    [SerializeField] Color32 Active;
    [SerializeField] Sprite ActiveImg;
    [Header("BuyPerkSettings")]
    public Image perkImage;
    public TextMeshProUGUI perkName;
    public TextMeshProUGUI statName;
    public TextMeshProUGUI statValue;
    public TextMeshProUGUI price;
    public Button perkBuyButton;
    public TextMeshProUGUI money;

    static PerkSystem instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        foreach (var perk in perks)
        {
            Perk button = perk.GetComponent<Perk>();
            //PlayerPrefs.DeleteAll();
            if (!PlayerPrefs.HasKey(button.statName))
            {
                PlayerPrefs.SetFloat(button.statName, 0);
            }
            if (PlayerPrefs.GetFloat(button.statName) > 0)
            {
                perk.perk.isActive = true;
                perk.perk.buyed = true;
            }
           
        }
        ReloadPerkData();
    }
    public void ReloadPerkData()
    {
        foreach (var perk in perks)
        {
            // Перевірка активації
            if (perk.perk.buyed)
            {
                perk.perk.isActive = IsPreviousLevelBought(perk) || perk.perk.row == 0;
                perk.BuyedPerksImg.gameObject.SetActive(true);

                // Активація наступного рівня для всіх рядків, якщо куплено перк в рядку 0
                if (perk.perk.row == 0)
                {
                    ActivateNextLevelForAllPerks(perk.perk.level);
                }
                // Активація наступного рівня для поточного рядка (окрім 6 рівня)
                else if (perk.perk.level < 6)
                {
                    ActivateNextLevel(perk);
                }
                // Активація 6 рівня для поточного рядка
                else
                {
                    ActivateLevel(perk, 6);
                }
            }

            // Встановити колір зображення
            perk.GetComponent<Image>().color = perk.perk.isActive ? Active : UnActive;
            perk.transform.GetChild(0).GetComponent<Image>().color = perk.perk.isActive ? ActivePerk : UnActivePerk;
        }
    }
    bool IsPreviousLevelBought(Perk perk)
    {
        return perk.perk.level >= 1 && perks.Any(p => p.perk.level == perk.perk.level - 1 && (p.perk.row == perk.perk.row || p.perk.row == 0) && p.perk.buyed);
    }

    void ActivateNextLevelForAllPerks(int currentLevel)
    {
        perks.Where(p => p.perk.level == currentLevel + 1).ToList().ForEach(p => p.perk.isActive = true);
    }

    void ActivateNextLevel(Perk perk)
    {
        perks.Find(p => p.perk.level == perk.perk.level + 1 && p.perk.row == perk.perk.row).perk.isActive = true;
    }

    void ActivateLevel(Perk perk, int targetLevel)
    {
        perks.Find(p => p.perk.row == perk.perk.row && p.perk.level == targetLevel).perk.isActive = true;
    }
    public static void ReloadPerkData_Static()
    {
        instance.ReloadPerkData();
    }
    public void SetDescription()
    {
        Perk tab = GetComponent<TabGroup>().tabButtons.Find(t => t == GetComponent<TabGroup>().selectedTab).GetComponent<Perk>();
        perkImage.sprite = tab.perkImage.sprite;
        perkImage.SetNativeSize();
        perkName.text = tab.GetComponent<TabButton>().tooltipText;
        statName.text = tab.statName;
        statValue.text = tab.statValue;
        price.text = tab.price;
        perkBuyButton.interactable = int.Parse(price.text) <= int.Parse(money.text)
            && tab.perk.isActive && !tab.perk.buyed ? true : false;
    }
}
