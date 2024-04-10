using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public TextMeshProUGUI characterName;
    public Image characterImage;
    public GameObject active;
    public LevelInfoLoad loader;
    private void Start()
    {
        loader = FindObjectOfType<LevelInfoLoad>();
    }
    public void SelectSlot()
    {
        CharacterSelect selected = transform.parent.GetComponentsInChildren<CharacterSelect>().FirstOrDefault(c => c.active && c.gameObject != gameObject);

        if (selected != null)
        {

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
                        if (data.ID == selected.transform.GetSiblingIndex())
                        {
                            selected.active.SetActive(false);
                            data.isEquiped = false;
                        }
                        if (data.ID == transform.GetSiblingIndex())
                        {
                            data.isEquiped = true;
                            active.SetActive(true);
                        }
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

            PlayerPrefs.SetInt("Character",transform.GetSiblingIndex());
            loader.LoadCharacterInfo();
        }
    }
}
