using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillLevelInfo : MonoBehaviour
{
    public CheckLevel level;
    public SavedLocationsData dataLevel;
    public Image percentImgDone;
    public DataHashing hash;
    public Transform objTransform;
    public GameObject levelInfoPanel;
    private void Awake()
    {
        hash = FindObjectOfType<DataHashing>();
        objTransform = transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectLevelID);
        GetComponent<Button>().onClick.AddListener(() => FindAnyObjectByType<GameManager>().OpenPanel(levelInfoPanel));

        level = FindObjectOfType<CheckLevel>();
        MenuController menu = GetComponent<MenuController>();
        dataLevel.percent = LoadObjectLevel(menu.sceneCount);
        dataLevel.countOfCount = LoadObjectLevelCount(menu.sceneCount);
        dataLevel.countOfCountMax = LoadObjectLevelCountOfCountMax(menu.sceneCount);

        dataLevel.IDLevel = menu.sceneCount;

        level.SaveInventory(dataLevel.IDLevel, dataLevel.percent);


    }
    void SelectLevelID()
    {
        levelInfoPanel.transform.GetChild(0).GetComponent<LevelInfoLoad>().levelID = dataLevel.IDLevel;
    }
    public int LoadObjectLevel(int objectID)
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

                if (data.IDLevel == objectID)
                {
                    return data.percent;
                }
            }
        }

        return 0;
    }
    public int LoadObjectLevelCount(int objectID)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string decode = hash.Decrypt(line);
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(decode);

                if (data.IDLevel == objectID)
                {
                    return data.countOfCount;
                }
            }
        }

        return 0;
    }
    public int LoadObjectLevelCountOfCountMax(int objectID)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string decode = hash.Decrypt(line);
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(decode);

                if (data.IDLevel == objectID)
                {
                    return data.countOfCountMax;
                }
            }
        }

        return 0;
    }
    public string LoadObjectLevelCountText(int objectID)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string decode = hash.Decrypt(line);
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(decode);

                if (data.IDLevel == objectID)
                {
                    return data.countOfCount.ToString() + "/" + data.countOfCountMax;
                }
            }
        }

        return null;
    }
}
