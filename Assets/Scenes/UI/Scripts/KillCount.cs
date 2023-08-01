using System.IO;
using TMPro;
using UnityEngine;

public class KillCount : MonoBehaviour
{
    public int score;
    public int enemyCount;
    public TextMeshProUGUI fpsText;
    float deltaTime = 0.0f;
    float updateInterval = 1.0f; // Інтервал оновлення в секундах
    float lastUpdateTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Перевірка, чи настав час оновлення виводу FPS
        if (Time.unscaledTime - lastUpdateTime > updateInterval)
        {
            float fps = 1.0f / Time.deltaTime;
            fpsText.text = "FPS: " + fps.ToString("0.");
            lastUpdateTime = Time.unscaledTime;
        }
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
}
