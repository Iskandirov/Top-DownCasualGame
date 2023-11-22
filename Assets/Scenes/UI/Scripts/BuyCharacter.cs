using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BuyCharacter : MonoBehaviour
{
    [SerializeField]
    private List<SavedCharacterData> characters = new List<SavedCharacterData>();
    //[SerializeField]
    //private List<SavedCharacterData> upgrades = new List<SavedCharacterData>();
    public List<SavedCharacterData> charactersRead = new List<SavedCharacterData>();

    public List<CharacterInfo> info;
    GetScore score;
    public bool isShopingScene;

    public DataHashing hashing;
    private void Awake()
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        if (!File.Exists(path))
        {
            SaveInventory();
        }
        SetParameters();
    }
    public void Start()
    {
        score = FindObjectOfType<GetScore>();
    }

    public void LoadInventory(List<SavedCharacterData> itemsRead)
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                // Розшифрувати JSON рядок
                string decryptedJson = hashing.Decrypt(jsonLine);

                SavedCharacterData data = JsonUtility.FromJson<SavedCharacterData>(decryptedJson);

                itemsRead.Add(data);
            }
        }
        else
        {
            File.Create(path);
        }
    }
    private void SaveInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            SavedCharacterData data = new SavedCharacterData();
            foreach (SavedCharacterData item in characters)
            {
                data.Name = item.Name;
                data.ID = item.ID;
                data.isBuy = item.isBuy;
                data.isEquiped = item.isEquiped;
                data.status = item.status;
                data.interactable = item.interactable;
                data.move = item.move;
                data.attackSpeed = item.attackSpeed;
                data.damage = item.damage;
                data.health = item.health;
                data.spell = item.spell;
                data.spellCD = item.spellCD;

                string jsonData = JsonUtility.ToJson(data);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    private void SaveUpgrade(int id)
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (SavedCharacterData item in charactersRead)
            {
                if (item.ID == id)
                {
                    item.isBuy = true;
                    item.status = "sold_out";
                }

                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    private void SaveEquip(int id)
    {
        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (SavedCharacterData item in charactersRead)
            {
                if (item.ID == id)
                {
                    item.isEquiped = true;
                    item.status = "choosen";
                    item.interactable = false;
                }
                else
                {
                    item.isEquiped = false;
                    if (item.isBuy)
                    {
                        item.status = "sold_out";
                        item.interactable = true;
                    }
                }

                string jsonData = JsonUtility.ToJson(item);
                string decryptedJson = hashing.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    public void BuyChoose(CharacterInfo info)
    {
        if (info.buyButton.tagText == "buy" && score.score >= info.price)
        {
            score.score = score.LoadScore() - info.price;
            score.SaveScore((int)score.score, false);
            score.scoreEnd.text = score.LoadScore().ToString();
            SaveUpgrade(info.id);
            SetParameters();
        }
        else if (info.buyButton.tagText == "sold_out")
        {
            SaveEquip(info.id);
            SetParameters();
        }
    }
    public void SetParameters()
    {
        charactersRead.Clear();
        LoadInventory(charactersRead);
        if (isShopingScene)
        {
            foreach (CharacterInfo character in info)
            {
                foreach (SavedCharacterData item in charactersRead)
                {
                    if (character.id == item.ID)
                    {
                        character.Name.text = item.Name;
                        character.buyButton.tagText = item.status;
                        character.damage.text = item.damage;
                        character.health.text = item.health;
                        character.spell.text = item.spell;
                        character.attackSpeed.text = item.attackSpeed;
                        character.move.text = item.move;
                        if (item.isBuy == true)
                        {
                            character.check.SetActive(item.isBuy);
                            character.priceObj.SetActive(!item.isBuy);
                            character.button.interactable = item.interactable;
                        }
                    }
                }

            }
        }
        
    }
}
