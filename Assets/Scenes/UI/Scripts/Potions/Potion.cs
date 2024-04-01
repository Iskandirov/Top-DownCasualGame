using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    public Image perkImage;
    public TextMeshProUGUI count;
    public TextMeshProUGUI price;
    public PotionBase potion;
    private void Start()
    {
        potion.count = PlayerPrefs.GetInt(potion.type.ToString());
        count.text = potion.count.ToString();
        price.text = potion.price.ToString();
    }
}
