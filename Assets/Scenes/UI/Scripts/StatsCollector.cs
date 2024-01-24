using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class StatsCollector : MonoBehaviour
{
    public List<Statistic> statRead;
    public static StatsCollector Instance;
    private void Awake()
    {
        Instance ??= this;
    }
    private void Start()
    {
        LoadScore();
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
                string decrypt = FindObjectOfType<DataHashing>().Decrypt(jsonLine);

                Statistic data = JsonUtility.FromJson<Statistic>(decrypt);
                statRead.Add(data);
            }
        }
    }
    public void FindStatName(string name,float value)
    {
        foreach(Statistic stat in statRead)
        {
            if (stat.Name == name)
            {
                stat.stat += value;
            }
        }
    }
}
