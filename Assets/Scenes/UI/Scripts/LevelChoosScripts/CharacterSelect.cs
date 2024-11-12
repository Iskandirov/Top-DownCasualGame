using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public TextMeshProUGUI characterName;
    public Image characterImage;
    public GameObject active;
    public LevelInfoLoad loader;
    public int charID;
    private void Start()
    {
        loader = FindObjectOfType<LevelInfoLoad>();
    }
    public void SelectSlot()
    {
        List<CharacterSelect> characters = transform.parent.GetComponentsInChildren<CharacterSelect>().ToList();

        string path = Path.Combine(Application.persistentDataPath, "CharacketInfo.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                foreach (string jsonLine in lines)
                {
                    string decryptedJson = DataHashing.inst.Decrypt(jsonLine);

                    SavedCharacterData data = JsonUtility.FromJson<SavedCharacterData>(decryptedJson);
                    foreach (var character in characters)
                    {
                        character.active.SetActive(false);
                    }
                    characters.FirstOrDefault(c => c.charID == charID).active.SetActive(true);
                    data.isEquiped = charID == data.ID;
                   
                    

                    //if (data.ID == charID)
                    //{
                    //    data.isEquiped = true;
                    //    active.SetActive(true);
                    //}
                    //if (data.ID != charID)
                    //{
                    //    Debug.Log(charID);
                    //    active.SetActive(false);
                    //    data.isEquiped = false;
                    //}
                    //if (data.ID == charID)
                    //{
                    //    data.isEquiped = true;
                    //    active.SetActive(true);
                    //}

                    string jsonData = JsonUtility.ToJson(data);
                    string encryptedJson = DataHashing.inst.Encrypt(jsonData);
                    writer.WriteLine(encryptedJson);
                }
                writer.Close();
            }
        }
        else
        {
            File.Create(path);
        }

        PlayerPrefs.SetInt("Character", charID);
        loader.LoadCharacterInfo();
    }
}
