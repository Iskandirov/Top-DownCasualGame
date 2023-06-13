using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillLevelInfo : MonoBehaviour
{
    public CheckLevel level;
    public SavedLocationsData dataLevel;
    public TextMeshProUGUI percent;
    public Image percentImg;
    // Start is called before the first frame update
    void Start()
    {
        level = FindObjectOfType<CheckLevel>();
        dataLevel.percent = LoadObjectLevel(gameObject.GetComponent<MenuController>().sceneCount);
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
        level.levels.Add(dataLevel);
        level.SaveInventory(dataLevel.IDLevel, dataLevel.percent);
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
