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

        // ��������� ������, �� �������� �� ��� ���� ���� ������
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

    // �������� ��������� ������
    void GenerateInitialFloor()
    {
        // ���������� ������� �����, �� ��������� �����������
        int floorCount = (int)(Camera.main.orthographicSize * 2 / floorHeight) + 2;

        // ���� ��� ��������� ��������� ������
        float x = 0f;
        float y = 0f;

        // �������� ������ �� ��������� ����� for
        for (int i = 0; i < floorCount; i++)
        {
            // ��������� ��������� ������� ������ �� ����
            GameObject floorInstance = Instantiate(floorPrefab, transform);

            // ������������ ������� ���������� ���������� ������� ������
            floorInstance.transform.position = new Vector3(x, y, 0f);

            // �������� �������� ����� x �� ������� floorWidth
            x += floorWidth;
            y += floorHeight;
        }
    }

    // �������� ���� ���������
    void GenerateFloor()
    {
        Debug.Log(1);
        currentIndex = (int)(playerTransform.position.x / floorWidth);
        if (playerTransform.position.x + minDistance > xOffset + currentIndex * floorWidth)
        {
            xOffset = (currentIndex + Random.Range(minDistance, maxDistance)) * floorWidth;
            yOffset = Random.Range(-floorHeight, floorHeight);

            // ��������� ��������� ������� ������ �� ����
            GameObject floorInstance = Instantiate(floorPrefab, transform);
            // ������������ ������� ���������� ���������� ������� ������
            floorInstance.transform.position = new Vector3(xOffset, playerTransform.position.y + yOffset, 0f);
        }
    }
}
