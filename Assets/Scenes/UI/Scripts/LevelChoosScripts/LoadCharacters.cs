using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadCharacters : MonoBehaviour
{
    [SerializeField] List<SavedCharacterData> itemsRead;
    [SerializeField] CharacterSelect characterPrefab;
    [SerializeField] DataHashing hashing;
    private void OnEnable()
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

        int offset = 220;
        RectTransform parentRect = GetComponent<RectTransform>();
        if (transform.childCount <= 1)
        {
            foreach (var character in itemsRead)
            {
                if (character.status != "buy")
                {
                    CharacterSelect example = Instantiate(characterPrefab, transform.parent);
                    example.transform.SetParent(transform, false);
                    example.transform.position = Vector3.zero;
                    example.transform.localPosition = new Vector3(-parentRect.offsetMin.x + offset, parentRect.offsetMin.y - 250, 0);
                    example.characterName.text = character.Name;
                    example.characterImage.sprite = Resources.Load<Sprite>(character.Name);
                    example.characterImage.SetNativeSize();
                    example.active.SetActive(character.isEquiped);
                    offset += 220;
                }
            }
        }
    }
   
}
