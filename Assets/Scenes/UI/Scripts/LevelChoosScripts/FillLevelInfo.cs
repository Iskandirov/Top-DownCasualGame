using System.IO;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillLevelInfo : MonoBehaviour
{
    public CheckLevel level;
    public SavedLocationsData dataLevel;
    public TextMeshProUGUI percent;
    public Image percentImg;
    public Image countImg;
    public TextMeshProUGUI countOfCount;
    public Image percentImgDone;
    public DataHashing hash;
    private void Awake()
    {
        hash = FindObjectOfType<DataHashing>();
    }
    // Start is called before the first frame update
    void Start()
    {
        level = FindObjectOfType<CheckLevel>();

        dataLevel.percent = LoadObjectLevel(GetComponent<MenuController>().sceneCount);
        dataLevel.countOfCount = LoadObjectLevelCount(GetComponent<MenuController>().sceneCount);

        percentImg.fillAmount = (float)dataLevel.percent / 100;
        countImg.fillAmount = (float)dataLevel.countOfCount / 5;

        if (dataLevel.percent == 100)
        {
            percentImgDone.gameObject.SetActive(true);
        }
        
        dataLevel.IDLevel = GetComponent<MenuController>().sceneCount;
        if (dataLevel.IDLevel == 999)
        {
            percent.gameObject.SetActive(false);
        }

        percent.text = dataLevel.percent.ToString() + "%";
        countOfCount.text = LoadObjectLevelCountText(GetComponent<MenuController>().sceneCount);
        level.SaveInventory(dataLevel.IDLevel, dataLevel.percent);


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
    public bool LoadObjectLevelCountIsFull(int objectID)
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
                    return data.isFullDone;
                }
            }
        }

        return false;
    }
}
