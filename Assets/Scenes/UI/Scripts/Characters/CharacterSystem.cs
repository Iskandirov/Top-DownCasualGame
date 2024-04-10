using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSystem : MonoBehaviour
{
    public Image characterImage;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI spell;
    public TextMeshProUGUI health;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI move;
    public TextMeshProUGUI attackSpeed;
    public TextMeshProUGUI description;
    public TagText buyButton;
    public TextMeshProUGUI price;
    public TextMeshProUGUI money;
    public Button button;
    public void SetDescription()
    {
        CharacterInfo tab = GetComponent<TabGroup>().tabButtons.Find(t => t == GetComponent<TabGroup>().selectedTab).GetComponent<CharacterInfo>();
        characterImage.sprite = tab.characterImage;
        characterImage.SetNativeSize();
        Name.text = tab.Name;
        spell.text = tab.spell;
        health.text = tab.health.ToString();
        damage.text = tab.damage.ToString();
        move.text = tab.move.ToString();
        attackSpeed.text = tab.attackSpeed.ToString();
        price.text = tab.price.ToString();
        description.text = tab.description;

        button.interactable = int.Parse(price.text) <= int.Parse(money.text) ? true : false;
    }
    public void BuyCharacter()
    {
        CharacterInfo character = GetComponent<TabGroup>().tabButtons.Find(t => t == GetComponent<TabGroup>().selectedTab).GetComponent<CharacterInfo>();
        money.text = GetScore.SaveMoney_Static(int.Parse(money.text) - int.Parse(price.text));
        GameManager.Instance.SaveCharacterUpgrade(character.id);
        PlayerPrefs.SetInt("Character", character.id);
        character.check.SetActive(true);
    }
}
