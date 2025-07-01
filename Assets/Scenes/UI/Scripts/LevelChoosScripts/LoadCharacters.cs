using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LoadCharacters : MonoBehaviour
{
    [SerializeField] List<SavedCharacterData> itemsRead;
    [SerializeField] CharacterSelect characterPrefab;
    [SerializeField] DataHashing hashing;
    private void OnEnable()
    {
        itemsRead.Clear();
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
        Debug.Log(transform.childCount);
        // Збираємо всі вже створені ID персонажів серед дочірніх об'єктів
        HashSet<int> existingIds = new HashSet<int>();
        foreach (Transform child in transform)
        {
            var charSelect = child.GetComponent<CharacterSelect>();
            if (charSelect != null)
                existingIds.Add(charSelect.charID);
        }

        foreach (var character in itemsRead)
        {
            if (character.status != "buy" && !existingIds.Contains(character.ID))
            {
                CharacterSelect example = Instantiate(characterPrefab, transform.parent);
                example.transform.SetParent(transform, false);
                example.transform.position = Vector3.zero;
                example.transform.localPosition = new Vector3(-parentRect.offsetMin.x + offset, parentRect.offsetMin.y - 250, 0);
                example.characterName.text = character.Name;
                example.characterImage.sprite = GameManager.ExtractSpriteListFromTexture("heroes").First(c => c.name == character.Name);
                example.charID = character.ID;
                example.characterImage.SetNativeSize();
                example.active.SetActive(character.isEquiped);
                offset += 220;
            }
        }
    }
    //private void OnDisable()
    //{
    //    foreach (Transform child in transform)
    //    {
    //        if (child.GetComponent<CharacterSelect>() != null)
    //        {
    //            Destroy(child.gameObject);
    //        }
    //    }
    //}
}
