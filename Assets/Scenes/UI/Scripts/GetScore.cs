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
                // ����� ������������
                score = (FindObjectOfType<KillCount>().score + 1) * Mathf.Pow(1 + (0.05f * result), 1.1f) + 1;
                percent = Mathf.RoundToInt((result / objTimer.timeToWin) * 100);
                if (percent > 100 && isWinPanel)
                {
                    percent = 100;
                }
                else
                {
                    percent = 99;
                }
                percentEnd.text = percent.ToString("0.") + "%";
            }
            else
            {
                // ������� ������������, ������� ������� ��� ������������ �������� �� �������������
                score = 0; // ���������, ������������ �������� �� �������������
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
            score = 1500;
            scoreEnd.text = score.ToString("0.");
            score += LoadScore();
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

            // ������ ������� ������ � ����� ����� �� ���������� �� ���������� � ������ sprites
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
            using (StreamWriter writer = new StreamWriter(path,false))
            {
                SavedEconomyData data = new SavedEconomyData();
                data.money = money;
                string jsonData = JsonUtility.ToJson(data);

                // ������� ��� ����� ������� � ����
                string encryptedJson = hash.Encrypt(jsonData);
                // �������� WriteLine �� Write
                writer.Write(encryptedJson);
                writer.Close();
            }
        }

        //// ���� ��� ���������� ������, ��������� ��� ��� ������� ���
        //if (isInGame && isWinPanel)
        //{
        //    string pathLocations = Path.Combine(Application.persistentDataPath, "Levels.txt");

        //    try
        //    {
        //        // ������� ���� �����
        //        string[] lines = File.ReadAllLines(pathLocations);

        //        // ��������� �������� ��� ������� id (���������, id = 1)
        //        int targetId = SceneManager.GetActiveScene().buildIndex;
        //        for (int i = 0; i < lines.Length; i++)
        //        {
        //            string decrypt = hash.Decrypt(lines[i]);
        //            SavedLocationsData dataLocations = JsonUtility.FromJson<SavedLocationsData>(decrypt);
        //            if (dataLocations.IDLevel == targetId && dataLocations.countOfCount < dataLocations.countOfCountMax)
        //            {
        //                dataLocations.countOfCount += 1;
        //                lines[i] = JsonUtility.ToJson(dataLocations);
        //                break; // ���� �������� �������� id, �������� � �����
        //            }
        //        }
        //        using (StreamWriter writer = new StreamWriter(pathLocations, false))
        //        {
        //            foreach (string item in lines)
        //            {
        //                string jsonData = JsonUtility.ToJson(item);
        //                string decryptedJson = hash.Encrypt(jsonData);
        //                writer.WriteLine(decryptedJson);
        //            }
        //            writer.Close();
        //        }
        //    }
        //    catch (IOException e)
        //    {
        //        Debug.LogError("Error writing to file: " + e.Message);
        //    }
        //}
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

}
