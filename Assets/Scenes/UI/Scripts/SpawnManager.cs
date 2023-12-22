using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    GameManager gameManager;
    PlayerManager player;
    public static SpawnManager inst;

    [Header("Barrels")]
    public int barrelPoolSize = 5;
    public List<GameObject> barrelPool;
    public GameObject barrelPrefab; // Префаб об'єкта, який потрібно спавнити
    public float barrelSpawnInterval = 20f; // Інтервал спавну в секундах

    [Header("Enemies")]
    public int enemyPoolSize = 60;
    public List<GameObject> enemyPool;
    public float enemySpawnInterval;
    public float timeStepWeed;
    public GameObject[] EnemyBody;
    public Collider2D spawnMapBound;
    public Timer Timer;
    public bool stopSpawn;
    private Camera mainCamera;
    public float spawnRadius = 10f;
    public float radiusWeed = 5.0f;
    private void Awake()
    {
        inst ??= this;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        player = PlayerManager.instance;

        barrelPool = InitializeObjectPool(barrelPrefab, barrelPoolSize);
        enemyPool = InitializeObjectPool(EnemyBody.ToList(), enemyPoolSize);
        StartCoroutine(SpawnRoutine(barrelSpawnInterval));
        StartCoroutine(SpawnEnemyRoutine(enemySpawnInterval));
    }
    private void FixedUpdate()
    {
        if (!stopSpawn)
        {
            mainCamera = Camera.main;
        }
    }
    List<GameObject> InitializeObjectPool(GameObject objectPrefab,int pool)
    {
        List<GameObject>  objectPool = new List<GameObject>();
        for (int i = 0; i < pool; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
        return objectPool;
    }
    List<GameObject> InitializeObjectPool(List<GameObject> objectPrefab, int pool)
    {
        List<GameObject>  objectPool = new List<GameObject>();
        int part = pool / objectPrefab.Count;
        int partNumber;
        for (int i = 0; i < pool; i++)
        {
            partNumber = (i / part) % objectPrefab.Count;
            GameObject obj = Instantiate(objectPrefab[partNumber]);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
        return objectPool;
    }
    GameObject GetFromPool(List<GameObject> objectPool,GameObject objectPrefab)
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }
        GameObject gameObject = Instantiate(objectPrefab);
        objectPool.Add(gameObject);
        return gameObject;
    }
    GameObject GetFromPool(List<GameObject> objectPool)
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }
        //GameObject gameObject = Instantiate(objectPrefab);
        //objectPool.Add(gameObject);
        return null;
    }
    private IEnumerator SpawnRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            GameObject barrel = GetFromPool(barrelPool, barrelPrefab);
            SpawnObjectInMapBounds(barrel);
        }
    }

    public void SpawnObjectInMapBounds(GameObject obj)
    {
        // Отримуємо центр колайдера
        Vector2 colliderCenter = spawnMapBound.bounds.center;
        Vector2 randomPointInsideCollider = colliderCenter + Random.insideUnitCircle * (spawnMapBound.bounds.extents.magnitude);
        obj.transform.position = randomPointInsideCollider;
    }
    //Enemy
    private IEnumerator SpawnEnemyRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            SpawnEnemies();
        }
    }
    private bool IsInsideCameraBounds(Vector3 position)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
        return viewportPosition.x >= 0f && viewportPosition.x <= 1f && viewportPosition.y >= 0f && viewportPosition.y <= 1f;
    }

    private bool IsInsideWallBounds(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f); // Використовуємо OverlapCircleAll замість OverlapPointAll
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition;
        do
        {
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * spawnRadius;
            spawnPosition = new Vector3(mainCamera.transform.position.x + spawnOffset.x, mainCamera.transform.position.y + spawnOffset.y, 1.8f);
        } while (IsInsideCameraBounds(spawnPosition) || IsInsideWallBounds(spawnPosition));

        return spawnPosition;
    }

    private void SpawnEnemies()
    {
        GameObject enemy = GetFromPool(enemyPool);
        IDChecker(enemy.name);

        if (enemy.GetComponent<Forward>() != null)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            enemy.transform.position = spawnPosition;
            enemy.transform.parent = transform;
            enemy.GetComponent<Forward>().player = player;
        }

        if (enemy.GetComponent<Forward>() == null && enemy.GetComponent<MoveToPlayerStartPos>() == null)
        {
            SpawnObjectInMapBounds(enemy);
        }
        if (enemy.GetComponent<MoveToPlayerStartPos>() != null)
        {
            if (timeStepWeed > 10)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                for (int y = 0; y < 10; y++)
                {
                    float angle = y * Mathf.PI * 2 / 10; // Розраховуємо кут між об'єктами
                    spawnPosition += new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radiusWeed; // Обчислюємо позицію для спавну
                    enemy.transform.position = spawnPosition; 
                    enemy.transform.parent = transform;
                }
                timeStepWeed = 0;
            }
        }
    }
    public void IDChecker(string enemyName)
    {
        foreach (SaveEnemyInfo obj in gameManager.enemyInfo)
        {
            if (obj.Name.Contains(enemyName))
            {
                if (gameManager.CheckInfo(obj.ID))
                {
                    gameManager.FillInfo(obj.ID);
                    gameManager.enemyInfoLoad.Clear();
                    gameManager.LoadEnemyInfo();
                }
            }
        }
    }
}
