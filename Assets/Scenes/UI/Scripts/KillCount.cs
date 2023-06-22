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
}
