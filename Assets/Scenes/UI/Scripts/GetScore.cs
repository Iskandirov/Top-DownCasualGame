using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
[DefaultExecutionOrder(10)]
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
    static GetScore instance;
    public float timeToSpawnBoss;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        hash = DataHashing.inst;
        Timer objTimer = FindObjectOfType<Timer>();
        if (GameManager.Instance.IsGamePage && !PlayerManager.instance.isTutor)
        {
            timeToSpawnBoss = EnemySpawner.instance.timeToSpawnBoss;
        }
        if (scoreEnd != null)
        {
            if (objTimer)
            {
                timeEnd.text = objTimer.time.ToString("00.00");
                if (!PlayerManager.instance.isTutor && DailyQuests.instance.quest.FirstOrDefault(s => s.id == 6 && s.isActive == true) != null)
                {
                    DailyQuests.instance.UpdateValue(6, objTimer.time - timeToSpawnBoss, true);
                }
                if (float.TryParse(timeEnd.text, out float result))
                {
                    // Вдале перетворення
                    score = (GameManager.Instance.score + 1) * Mathf.Pow(1 + (0.05f * result), 1.1f) + 1;
                    percent = Mathf.RoundToInt((result / timeToSpawnBoss) * 100);
                    if (percent >= 100 && isWinPanel)
                    {
                        percent = 100;
                    }
                    else if (percent >= 100 && !isWinPanel)
                    {
                        percent = 99;
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
            if (FindObjectOfType<Tutor>() != null)
            {
                percent = 100;
                percentEnd.text = percent.ToString("0.") + "%";
                score = 1500;
                scoreEnd.text = score.ToString("0.");
                score += LoadScore();
            }
        }
    }
    void Start()
    {
        if (scene)
        {
            SaveScore((int)score);
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
    public void SaveScore(int money)
    {
            string path = Path.Combine(Application.persistentDataPath, "Economy.txt");

            if (File.Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path, false))
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
    }
    public void SaveScore()
    {
        string path = Path.Combine(Application.persistentDataPath, "Economy.txt");
        SavedEconomyData data = new SavedEconomyData();

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            string jsonData = JsonUtility.ToJson(data);
            string decryptedJson = hash.Encrypt(jsonData);
            writer.WriteLine(decryptedJson);
            writer.Close();
        }
    }
    public static string SaveMoney_Static(int money)
    {
        instance.SaveScore(money);
        return instance.LoadScore().ToString();
    }
}
