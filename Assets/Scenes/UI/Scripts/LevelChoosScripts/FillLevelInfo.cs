using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillLevelInfo : MonoBehaviour
{
    public CheckLevel level;
    public SavedLocationsData dataLevel;
    public TextMeshProUGUI percent;
    public TextMeshProUGUI countOfCount;
    public Image percentImg;
    // Start is called before the first frame update
    void Start()
    {
        level = FindObjectOfType<CheckLevel>();

        dataLevel.percent = LoadObjectLevel(gameObject.GetComponent<MenuController>().sceneCount);
        dataLevel.countOfCount = LoadObjectLevelCount(gameObject.GetComponent<MenuController>().sceneCount);

        if(dataLevel.percent == 100)
        {
            percentImg.gameObject.SetActive(true);
        }
        
        dataLevel.IDLevel = gameObject.GetComponent<MenuController>().sceneCount;
        if (dataLevel.IDLevel == 999)
        {
            percent.gameObject.SetActive(false);
        }

        percent.text = LoadObjectLevel(gameObject.GetComponent<MenuController>().sceneCount).ToString() + "%";
        countOfCount.text = LoadObjectLevelCountText(gameObject.GetComponent<MenuController>().sceneCount);

        level.SaveInventory(dataLevel.IDLevel, dataLevel.percent);
        
    }

    public int LoadObjectLevel(int objectID)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels.txt");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(lines[i]);

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

            for (int i = 0; i < lines.Length; i++)
            {
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(lines[i]);

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

            for (int i = 0; i < lines.Length; i++)
            {
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(lines[i]);

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

            for (int i = 0; i < lines.Length; i++)
            {
                SavedLocationsData data = JsonUtility.FromJson<SavedLocationsData>(lines[i]);

                if (data.IDLevel == objectID)
                {
                    return data.isFullDone;
                }
            }
        }

        return false;
    }
}
