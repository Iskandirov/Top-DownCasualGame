using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public Transform playerTransform;
    public float floorWidth = 10f;
    public float floorHeight = 3f;
    public float minDistance = 5f;
    public float maxDistance = 10f;
    public int maxFloorCount = 20;

    private float xOffset = 0f;
    private float yOffset = 0f;
    private int currentIndex = 0;
    [SerializeField]
    private int floorCount = 0;

    void Start()
    {
        GenerateInitialFloor();
        currentIndex = (int)(playerTransform.position.x / floorWidth);
    }

    void Update()
    {
        if (floorCount < maxFloorCount)
        {
            GenerateFloor();
        }

        // Видаляємо підлоги, які виходять за межі поля зору гравця
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.position.x < (playerTransform.position.x - maxDistance))
            {
                Destroy(child.gameObject);
                floorCount--;
            }
        }
    }

    // Генеруємо початкову підлогу
    void GenerateInitialFloor()
    {
        // Обчислюємо кількість підлог, які необхідно згенерувати
        int floorCount = (int)(Camera.main.orthographicSize * 2 / floorHeight) + 2;

        // Змінні для зберігання координат підлоги
        float x = 0f;
        float y = 0f;

        // Генеруємо підлогу за допомогою циклу for
        for (int i = 0; i < floorCount; i++)
        {
            // Створюємо екземпляр префабу підлоги на сцені
            GameObject floorInstance = Instantiate(floorPrefab, transform);

            // Встановлюємо позицію створеного екземпляру префабу підлоги
            floorInstance.transform.position = new Vector3(x, y, 0f);

            // Збільшуємо значення змінної x на відстань floorWidth
            x += floorWidth;
            y += floorHeight;
        }
    }

    // Генеруємо нову платформу
    void GenerateFloor()
    {
        Debug.Log(1);
        currentIndex = (int)(playerTransform.position.x / floorWidth);
        if (playerTransform.position.x + minDistance > xOffset + currentIndex * floorWidth)
        {
            xOffset = (currentIndex + Random.Range(minDistance, maxDistance)) * floorWidth;
            yOffset = Random.Range(-floorHeight, floorHeight);

            // Створюємо екземпляр префабу підлоги на сцені
            GameObject floorInstance = Instantiate(floorPrefab, transform);
            // Встановлюємо позицію створеного екземпляру префабу підлоги
            floorInstance.transform.position = new Vector3(xOffset, playerTransform.position.y + yOffset, 0f);
        }
    }
}
