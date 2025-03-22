using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoLoad : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelName;
    [SerializeField] Image levelImage;
    [SerializeField] TextMeshProUGUI countOfCount;
    [SerializeField] TextMeshProUGUI levelDescription;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] Image playerImage;
    [SerializeField] TextMeshProUGUI healthValue;
    [SerializeField] TextMeshProUGUI damageValue;
    [SerializeField] TextMeshProUGUI speedValue;
    [SerializeField] TextMeshProUGUI attackSpeedValue;
    [SerializeField] TextMeshProUGUI specialAttack;
    [SerializeField] MenuController playButton;
    [SerializeField] DataHashing hash;
    public int levelID;
    void OnEnable()
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string decode = hash.Decrypt(line); // Розшифровуємо рядок даних

                // Тут ми працюємо з розшифрованим JSON рядком
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(decode);
                if (data.IDLevel == levelID)
                {
                    //levelName.text = data.name;
                    levelName.GetComponent<TagText>().tagText = "level_name_" + levelID;
                    countOfCount.text = data.countOfCount + "/" + data.countOfCountMax;
                    //levelDescription.text = data.description;
                    //specialAttack.GetComponent<TagText>().tagText = "base_spell_" + levelID;
                    levelDescription.GetComponent<TagText>().tagText = "description_lvl_" + levelID;
                    levelImage.sprite = GameManager.ExtractSpriteListFromTexture("Map").First(s => s.name == levelID.ToString());
                    break;
                }
            }
            LoadCharacterInfo();
        }
        playButton.sceneCount = levelID;
        GameManager.Instance.UpdateText(GameManager.Instance.texts);
    }
    public void LoadCharacterInfo()
    {
        SavedCharacterData character = GameManager.Instance.charactersRead.Find(ch => ch.ID == PlayerPrefs.GetInt("Character"));
        playerName.text = character.Name;
        healthValue.text = character.health;
        damageValue.text = character.damage;
        speedValue.text = character.move;
        attackSpeedValue.text = character.attackSpeed;
        specialAttack.GetComponent<TagText>().tagText = "base_spell_" + character.ID;
        //specialAttack.text = character.spell;
        playerImage.sprite = GameManager.ExtractSpriteListFromTexture("heroes").First(s => s.name == character.Name);
    }
}