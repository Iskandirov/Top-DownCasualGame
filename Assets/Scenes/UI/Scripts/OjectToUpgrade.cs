using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OjectToUpgrade : MonoBehaviour
{
    public GameObject objList;
    public TextMeshProUGUI tooltip;
    public TextMeshProUGUI money;
    public GetScore setMoney;
    public int objID;
    public int price;
    // Start is called before the first frame update
    void Start()
    {
        MathPrice();
    }

    public void MathPrice()
    {
        foreach (var img in objList.GetComponent<UpgradeObjInfo>().itemsRead)
        {
            if (img.IDObject == objID)
            {
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(img.name + "_" + img.levelUpgrade);
                price = (int)(img.price * Mathf.Pow(2.71828f, (1.2f * img.levelUpgrade)));
                tooltip.text = price.ToString();
            }
        }
    }
    public void Upgrade()
    {
        if (int.TryParse(tooltip.text, out int priceRes) && int.TryParse(money.text, out int moneyRes))
        {
            if (moneyRes >= priceRes)
            {
                moneyRes -= priceRes;
                money.text = moneyRes.ToString();
                setMoney.SaveScore(moneyRes);
                foreach (var img in objList.GetComponent<UpgradeObjInfo>().itemsRead)
                {
                    if (img.IDObject == objID)
                    {
                        objList.GetComponent<UpgradeObjInfo>().SaveInventory(img.levelUpgrade + 1, objID);
                        objList.GetComponent<UpgradeObjInfo>().itemsRead.Clear();
                        objList.GetComponent<UpgradeObjInfo>().LoadInventory(objList.GetComponent<UpgradeObjInfo>().itemsRead);
                        MathPrice();
                        break;
                    }
                }
                setMoney.LoadScore();
            }
        }
    }
}
