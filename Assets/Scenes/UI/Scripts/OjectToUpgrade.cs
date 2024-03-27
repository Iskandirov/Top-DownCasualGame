using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OjectToUpgrade : MonoBehaviour
{
    //public GameObject objList;
    //public TextMeshProUGUI tooltip;
    //public TextMeshProUGUI money;
    //public GetScore setMoney;
    //public int objID;
    //public int price;
    //UpgradeObjInfo objInfo;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    objInfo = objList.GetComponent<UpgradeObjInfo>();
    //    MathPrice();
    //}

    //public void MathPrice()
    //{
    //    foreach (var img in objInfo.itemsRead)
    //    {
    //        if (img.IDObject == objID)
    //        {
    //            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(img.name + "_" + img.levelUpgrade);
    //            price = (int)(img.price * Mathf.Pow(2.71828f, (1.2f * img.levelUpgrade)));
    //            tooltip.text = price.ToString();
    //        }
    //    }
    //}
    //public void Upgrade()
    //{
    //    if (int.TryParse(tooltip.text, out int priceRes) && int.TryParse(money.text, out int moneyRes))
    //    {
    //        if (moneyRes >= priceRes)
    //        {
    //            moneyRes -= priceRes;
    //            money.text = moneyRes.ToString();
    //            setMoney.SaveScore(moneyRes);
    //            foreach (var img in objInfo.itemsRead)
    //            {
    //                if (img.IDObject == objID)
    //                {
    //                    objInfo.SaveInventory(img.levelUpgrade + 1, objID);
    //                    objInfo.itemsRead.Clear();
    //                    objInfo.LoadInventory(objInfo.itemsRead);
    //                    MathPrice();
    //                    break;
    //                }
    //            }
    //            setMoney.LoadScore();
    //        }
    //    }
    //}
   
}
