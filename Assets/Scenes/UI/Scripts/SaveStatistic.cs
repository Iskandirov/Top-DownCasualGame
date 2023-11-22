using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveStatistic : MonoBehaviour
{
    DataHashing hash;
    public List<Statistic> stat;
    public List<Statistic> statRead;
    public List<TextMeshProUGUI> nameStat;
    public List<TextMeshProUGUI> statText;
    public bool isInStatScene;
    public bool isInGame;
    // Start is called before the first frame update
    void Start()
    {
        hash = FindObjectOfType<DataHashing>();
        string path = Path.Combine(Application.persistentDataPath, "Statistic.txt");
        if (!File.Exists(path))
        {
            SaveStat();
        }
        LoadScore();
        if (isInStatScene)
        {
            SetStats();
        }
        else if (isInGame)
        {
            SaveStat(FindObjectOfType<StatsCollector>().statRead);
        }
    }
    public void LoadScore()
    {
        statRead.Clear();
        string path = Path.Combine(Application.persistentDataPath, "Statistic.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string jsonLine in lines)
            {
                string decrypt = hash.Decrypt(jsonLine);

                Statistic data = JsonUtility.FromJson<Statistic>(decrypt);
                statRead.Add(data);
            }
        }
    }
    public void SaveStat()
    {
        string path = Path.Combine(Application.persistentDataPath, "Statistic.txt");
        Statistic data = new Statistic();

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (var stat in stat)
            {
                data.ID = stat.ID;
                data.Name = stat.Name;
                data.stat = stat.stat;
                string jsonData = JsonUtility.ToJson(data);
                string decryptedJson = hash.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
     public void SaveStat(List<Statistic> data)
    {
        string path = Path.Combine(Application.persistentDataPath, "Statistic.txt");

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (var stat in data)
            {
                string jsonData = JsonUtility.ToJson(stat);
                string decryptedJson = hash.Encrypt(jsonData);
                writer.WriteLine(decryptedJson);
            }
            writer.Close();
        }
    }
    public void SetStats()
    {
        int count = 0;
        foreach (var item in nameStat)
        {
            item.text = statRead[count].Name;
            statText[count].text = statRead[count].stat.ToString("0.0");
            count++;
        }
    }
}
