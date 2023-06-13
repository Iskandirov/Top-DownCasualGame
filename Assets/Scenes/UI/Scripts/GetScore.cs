using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetScore : MonoBehaviour
{
    public TextMeshProUGUI timeEnd;
    public TextMeshProUGUI percentEnd;
    public TextMeshProUGUI scoreEnd;
    public float score;
    public int percent;
    public bool scene;
    // Start is called before the first frame update
    private void Awake()
    {
        if (FindObjectOfType<Timer>())
        {
            timeEnd.text = FindObjectOfType<Timer>().time.ToString("00.00");
            if (float.TryParse(timeEnd.text, out float result))
            {
                // Вдале перетворення
                score = ((FindObjectOfType<KillCount>().score + 1) * Mathf.Pow(2.71828f, (0.05f * result))) * 2;
                percent = Mathf.RoundToInt((result / FindObjectOfType<Timer>().timeToWin) * 100);
                percentEnd.text = percent.ToString("0.") + "%";
            }
            else
            {
                // Невдале перетворення, обробка помилки або встановлення значення за замовчуванням
                score = 0; // Наприклад, встановлення значення за замовчуванням
            }
            scoreEnd.text = score.ToString("0.");
            score += LoadScore();
        }
        else
        { 
            score += LoadScore();
            scoreEnd.text = score.ToString("0.");
        }
    }
    void Start()
    {
        SaveScore((int)score);
        if (scene)
        {
            gameObject.GetComponent<CheckLevel>().CheckPercent(SceneManager.GetActiveScene().buildIndex, percent);
        }
    }
    public int LoadScore()
    {
        int money = 0;
        string path = Path.Combine(Application.persistentDataPath, "Economy.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                SavedEconomyData data = JsonUtility.FromJson<SavedEconomyData>(jsonLine);

                money = data.money;
            }
            return money;
        }
        else
        {
            File.Create(path);
            return money;
        }
    }
    public void SaveScore(int money)
    {
        string path = Path.Combine(Application.persistentDataPath, "Economy.txt");

        SavedEconomyData data = new SavedEconomyData();
        data.money = money;

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(path, jsonData);
    }
    public void SaveScore()
    {
        string path = Path.Combine(Application.persistentDataPath, "Economy.txt");

        SavedEconomyData data = new SavedEconomyData();

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(path, jsonData);
    }
    
}
