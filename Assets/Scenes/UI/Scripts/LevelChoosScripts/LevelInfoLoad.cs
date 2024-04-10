using System.IO;
using System.Linq;
using System.Security.Policy;
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
                    levelName.text = data.name;
                    countOfCount.text = data.countOfCount + "/" + data.countOfCountMax;
                    levelDescription.text = data.description;
                    levelImage.sprite = Resources.Load<Sprite>("loh");
                    break;
                }
            }
            LoadCharacterInfo();
        }
        playButton.sceneCount = levelID;
    }
    public void LoadCharacterInfo()
    {
        SavedCharacterData character = GameManager.Instance.charactersRead.Find(ch => ch.ID == PlayerPrefs.GetInt("Character"));
        playerName.text = character.Name;
        healthValue.text = character.health;
        damageValue.text = character.damage;
        speedValue.text = character.move;
        attackSpeedValue.text = character.attackSpeed;
        specialAttack.text = character.spell;
        playerImage.sprite = Resources.Load<Sprite>(character.Name);
    }
}