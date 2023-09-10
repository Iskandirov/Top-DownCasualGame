using System.Linq;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public float timeStep;
    public float timeStepWeed;
    public float timeToNewType;
    public float timeToNewTypeStart;
    public int enemyTypeSpawn;
    [SerializeField] float time;
    [SerializeField] float timeGost;
    public GameObject[] EnemyBody;
    public MoveToPlayerStartPos tumbleweed;
    public SnipetreeAttack sniperTree;
    public Collider2D spawnMapBound;
    public Timer Timer;
    public bool stopSpawn;
    //public KillCount countEnemy;

    private Camera mainCamera;
    public float spawnRadius = 10f;
    public float enemyCountMax;
    public float[] enemyCountType;

    Transform player;

    public float radius = 5.0f;
    public EnemyInfoLoader enemyInfo;

    private void Start()
    {
        enemyCountType = new float[EnemyBody.Length];
        timeToNewType = timeToNewTypeStart;
        player = FindObjectOfType<Move>().transform;
    }

    private void FixedUpdate()
    {
        mainCamera = Camera.main;
        time += Time.deltaTime;
        timeStepWeed += Time.deltaTime;
        if (time >= timeStep && !stopSpawn)
        {
            ActivateSpawners();
        }
    }

    private Bounds GetCameraBounds()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;

        float minX = cameraPosition.x - cameraWidth / 2f;
        float maxX = cameraPosition.x + cameraWidth / 2f;
        float minY = cameraPosition.y - cameraHeight / 2f;
        float maxY = cameraPosition.y + cameraHeight / 2f;

        return new Bounds(cameraPosition, new Vector3(cameraWidth, cameraHeight, 0f));
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

    private Vector3 GetRandomSpawnPosition(Bounds cameraBounds)
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

    private void SpawnTumbleweed()
    {
        if (timeStepWeed > 10)
        {
            if (enemyInfo.CheckInfo(enemyTypeSpawn))
            {
                enemyInfo.FillInfo(enemyTypeSpawn);
                enemyInfo.enemyInfoLoad.Clear();
                enemyInfo.LoadEnemyInfo();
            }
            Bounds cameraBounds = GetCameraBounds();
            Vector3 spawnPosition = GetRandomSpawnPosition(cameraBounds);
            for (int i = 0; i < 10; i++)
            {
                float angle = i * Mathf.PI * 2 / 10; // Розраховуємо кут між об'єктами
                spawnPosition += new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius; // Обчислюємо позицію для спавну

                MoveToPlayerStartPos a = Instantiate(tumbleweed, spawnPosition, Quaternion.identity, transform);
                a.target = player;
            }
            timeStepWeed = 0;
            enemyCountType[1] += 1;
        }
    }
    private void SpawnInWorldBound(GameObject obj, int id)
    {
        if (enemyCountType[id] <= 5)
        {
            // Отримуємо центр колайдера
            Vector2 colliderCenter = spawnMapBound.bounds.center;

            // Отримуємо випадкову точку всередині колайдера за допомогою Random.insideUnitCircle
            Vector2 randomPointInsideCollider = colliderCenter + Random.insideUnitCircle * (spawnMapBound.bounds.extents.magnitude);

            // Спавнуємо об'єкт на отриманій позиції
            Instantiate(obj, randomPointInsideCollider, Quaternion.identity);
            enemyCountType[id] += 1;
        }
    }
    public void ActivateSpawners()
    {
        if (stopSpawn)
        {
            ResetMobCount();
        }
        int i = Random.Range(0, enemyTypeSpawn);
        switch (i)
        {
            case 0:
                SpawnEnemies(0, 20);
                break;
            case 1:
                SpawnTumbleweed();
                break;
            case 2:
                SpawnEnemies(2, 15);
                break;
            case 3:
                SpawnEnemies(3, 10);
                break;
            case 4:
                SpawnInWorldBound(EnemyBody[5], 4);
                break;
            case 5:
                SpawnInWorldBound(sniperTree.gameObject, 5);
                break;
            default:
                break;
        }
        if (Timer.time >= timeToNewType && enemyTypeSpawn < EnemyBody.Length)
        {
            timeToNewType += timeToNewTypeStart;
            enemyTypeSpawn++;
        }
        time = 0;
    }
    private void SpawnEnemies(int i, int maxCount)
    {
        if (enemyCountType[i] <= maxCount)
        {

            Bounds cameraBounds = GetCameraBounds();
            if (enemyCountType[i] < enemyCountMax)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition(cameraBounds);
                Instantiate(EnemyBody[i], spawnPosition, Quaternion.identity, transform);
                enemyCountType[i] += 1;
                if (enemyInfo.CheckInfo(enemyTypeSpawn))
                {
                    enemyInfo.FillInfo(enemyTypeSpawn);
                    enemyInfo.enemyInfoLoad.Clear();
                    enemyInfo.LoadEnemyInfo();
                }
            }
        }
    }
    public void ResetMobCount()
    {
        for (int i = 0; i < enemyCountType.Length; i++)
        {
            enemyCountType[i] = 0;
        }
    }
    public void SpawnEnemies(byte opacity, float speed, int health, float damage)
    {
        Bounds cameraBounds = GetCameraBounds();
        Vector3 spawnPosition = GetRandomSpawnPosition(cameraBounds);
        timeGost += Time.deltaTime;
        if (timeGost >= timeStep)
        {
            int i = Random.Range(0, EnemyBody.Length);
            GameObject enemy = Instantiate(EnemyBody[i], spawnPosition, Quaternion.identity);
            enemy.GetComponentInChildren<SpriteRenderer>().color = new Color32(255, 255, 255, opacity);
            if (enemy.GetComponent<Forward>() != null)
            {
                enemy.GetComponent<Forward>().speed *= speed;
            }
            if (enemy.GetComponent<Attack>() != null)
            {
                enemy.GetComponent<Attack>().damage *= damage;
            }
            enemy.GetComponentInChildren<HealthPoint>().healthPoint = health;
            timeGost = 0;
        }
    }
}
