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
    public TextMeshProUGUI price;
    public TextMeshProUGUI money;
    public Button button;
    public void SetDescription()
    {
        CharacterInfo tab = GetComponent<TabGroup>().tabButtons.Find(t => t == GetComponent<TabGroup>().selectedTab).GetComponent<CharacterInfo>();
        characterImage.sprite = tab.characterImage;
        characterImage.SetNativeSize();
        Name.text = tab.Name;
        spell.GetComponent<TagText>().tagText = "base_spell_" + tab.id;
        description.GetComponent<TagText>().tagText = "character_description_" + tab.id;
        //spell.text = tab.spell;
        health.text = tab.health.ToString();
        damage.text = tab.damage.ToString();
        move.text = tab.move.ToString();
        attackSpeed.text = tab.attackSpeed.ToString();
        price.text = tab.price.ToString();
        //description.text = tab.description;
       
        button.interactable = int.Parse(price.text) <= int.Parse(money.text) && !tab.check.activeSelf ? true : false;
        GameManager.Instance.UpdateText(GameManager.Instance.texts);
    }
    public void BuyCharacter()
    {
        CharacterInfo character = GetComponent<TabGroup>().tabButtons.Find(t => t == GetComponent<TabGroup>().selectedTab).GetComponent<CharacterInfo>();
        Debug.Log(character);
        money.text = GetScore.SaveMoney_Static(int.Parse(money.text) - int.Parse(price.text));
        GameManager.Instance.SaveCharacterUpgrade(character.id);
        PlayerPrefs.SetInt("Character", character.id);
        character.check.SetActive(true);
        button.interactable = false;
        GameManager.Instance.LoadInventory(GameManager.Instance.itemsRead);
    }
}
