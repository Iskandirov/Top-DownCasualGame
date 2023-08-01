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
    public bool isWinPanel;
    // Start is called before the first frame update
    private void Awake()
    {
        Timer objTimer = FindObjectOfType<Timer>();
        if (objTimer)
        {
            timeEnd.text = objTimer.time.ToString("00.00");
            if (float.TryParse(timeEnd.text, out float result))
            {
                // Вдале перетворення
                score = ((FindObjectOfType<KillCount>().score + 1) * Mathf.Pow(2.71828f, (0.05f * result))) * 2;
                percent = Mathf.RoundToInt((result / objTimer.timeToWin) * 100);
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
        SaveScore((int)score,true);
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
    public void SaveScore(int money, bool isInGame)
    {
        string path = Path.Combine(Application.persistentDataPath, "Economy.txt");

        SavedEconomyData data = new SavedEconomyData();
        data.money = money;

        string jsonData = JsonUtility.ToJson(data);

        try
        {
            // Перезаписуємо весь файл з новим рядком даних
            File.WriteAllText(path, jsonData);
        }
        catch (IOException e)
        {
            Debug.LogError("Error writing to file: " + e.Message);
        }

        if (isInGame && isWinPanel)
        {
            string pathLocations = Path.Combine(Application.persistentDataPath, "Levels.txt");

            try
            {
                // Зчитуємо вміст файлу
                string[] lines = File.ReadAllLines(pathLocations);

                // Оновлюємо значення для певного id (наприклад, id = 1)
                int targetId = SceneManager.GetActiveScene().buildIndex;
                for (int i = 0; i < lines.Length; i++)
                {
                    SavedLocationsData dataLocations = JsonUtility.FromJson<SavedLocationsData>(lines[i]);
                    if (dataLocations.IDLevel == targetId && dataLocations.countOfCount < 5)
                    {
                        dataLocations.countOfCount += 1;
                        lines[i] = JsonUtility.ToJson(dataLocations);
                        break; // Якщо знайдено відповідне id, виходимо з циклу
                    }
                }

                // Записуємо оновлений вміст назад у файл
                File.WriteAllLines(pathLocations, lines);
            }
            catch (IOException e)
            {
                Debug.LogError("Error writing to file: " + e.Message);
            }
        }
    }

    public void SaveScore()
    {
        string path = Path.Combine(Application.persistentDataPath, "Economy.txt");

        SavedEconomyData data = new SavedEconomyData();

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(path, jsonData);
    }
    
}
