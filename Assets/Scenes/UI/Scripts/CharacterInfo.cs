using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public int id;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI health;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI move;
    public TextMeshProUGUI attackSpeed;
    public TextMeshProUGUI spell;
    public TagText buyButton;
    public GameObject priceObj;
    public GameObject check;
    public Button button;
    public int price;
    public void Start()
    {
        if (int.TryParse(priceObj.GetComponent<TextMeshProUGUI>().text, out int result))
        {
            price = result;
        }
    }
}
