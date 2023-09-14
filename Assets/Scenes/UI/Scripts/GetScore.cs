using Newtonsoft.Json;
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
    DataHashing hash;
    // Start is called before the first frame update
    private void Awake()
    {
        hash = FindObjectOfType<DataHashing>();
        Timer objTimer = FindObjectOfType<Timer>();
        if (objTimer)
        {
            timeEnd.text = objTimer.time.ToString("00.00");
            if (float.TryParse(timeEnd.text, out float result))
            {
                // Вдале перетворення
                score = (FindObjectOfType<KillCount>().score + 1) * Mathf.Pow(1 + (0.05f * result), 1.1f);
                percent = Mathf.RoundToInt((result / objTimer.timeToWin) * 100);
                if (percent > 100)
                {
                    percent = 100;
                }
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
        SaveScore((int)score, true);
        if (scene)
        {
            GetComponent<CheckLevel>().CheckPercent(SceneManager.GetActiveScene().buildIndex, percent);
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
                string decrypt = hash.Decrypt(jsonLine);

                SavedEconomyData data = JsonUtility.FromJson<SavedEconomyData>(decrypt);

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

        if (File.Exists(path))
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                SavedEconomyData data = new SavedEconomyData();
                data.money = money;
                string jsonData = JsonUtility.ToJson(data);

                // Шифруємо дані перед записом у файл
                string encryptedJson = hash.Encrypt(jsonData);

                // Заміняємо WriteLine на Write
                writer.Write(encryptedJson);
                writer.Close();
            }
        }

        // Якщо гра закінчилася успішно, оновлюємо дані про пройдені рівні
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
                    string decrypt = hash.Decrypt(lines[i]);
                    SavedLocationsData dataLocations = JsonUtility.FromJson<SavedLocationsData>(decrypt);
                    if (dataLocations.IDLevel == targetId && dataLocations.countOfCount < 5)
                    {
                        dataLocations.countOfCount += 1;
                        lines[i] = JsonUtility.ToJson(dataLocations);
                        break; // Якщо знайдено відповідне id, виходимо з циклу
                    }
                }

                // Тепер шифруємо та зберігаємо змінені дані
                string jsonContent = string.Join("\n", lines);
                string encryptedJson = hash.Encrypt(jsonContent);
                File.WriteAllText(pathLocations, encryptedJson);
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
        string decryptedJson = hash.Encrypt(jsonData);
        File.WriteAllText(path, decryptedJson);
    }

}
