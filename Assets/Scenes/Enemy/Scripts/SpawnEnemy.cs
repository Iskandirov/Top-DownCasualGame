using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public float timeStep;
    public float timeToNewType;
    public float timeToNewTypeStart;
    public int enemyTypeSpawn;
    [SerializeField] float time;
    [SerializeField] float timeGost;
    public GameObject[] EnemyBody;
    public Timer Timer;
    public bool stopSpawn;
    public KillCount countEnemy;

    private Camera mainCamera;
    public float spawnRadius = 10f;
    public float enemyCountMax = 10f;
    void Start()
    {

        timeToNewType = timeToNewTypeStart;
    }

    void FixedUpdate()
    {
        mainCamera = Camera.main;
        SpawnEnemies();
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

    private bool IsInsideCameraBounds(Vector3 position, Bounds cameraBounds)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
        return viewportPosition.x >= 0f && viewportPosition.x <= 1f && viewportPosition.y >= 0f && viewportPosition.y <= 1f &&
               position.x >= cameraBounds.min.x && position.x <= cameraBounds.max.x &&
               position.y >= cameraBounds.min.y && position.y <= cameraBounds.max.y;
    }

    private bool IsInsideWallBounds(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
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
        } while (IsInsideCameraBounds(spawnPosition, cameraBounds) || IsInsideWallBounds(spawnPosition));

        return spawnPosition;
    }


    private void SpawnEnemies()
    {
        time += Time.deltaTime;
        Bounds cameraBounds = GetCameraBounds();

        if (time >= timeStep && !stopSpawn && countEnemy.enemyCount <= enemyCountMax)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(cameraBounds);
            int i = Random.Range(0, enemyTypeSpawn);
            Instantiate(EnemyBody[i], spawnPosition, Quaternion.identity, transform);
            countEnemy.enemyCount += 1;
            time = 0;
        }

        if (Timer.time >= timeToNewType && enemyTypeSpawn < EnemyBody.Length)
        {
            timeToNewType += timeToNewTypeStart;
            enemyTypeSpawn++;
        }
    }
    //private Vector3 GetRandomSpawnPosition()
    //{
    //    Vector3 randomPosition = new Vector3(
    //        Random.Range(spawnAreaMin.x, spawnAreaMax.x),
    //        Random.Range(spawnAreaMin.y, spawnAreaMax.y),
    //        1.8f);

    //    while (IsPositionWithinCameraBounds(randomPosition))
    //    {
    //        randomPosition = new Vector3(
    //            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
    //            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
    //            1.8f);
    //    }

    //    return randomPosition;
    //}

    //bool IsPositionWithinCameraBounds(Vector3 position)
    //{
    //    Camera mainCamera = Camera.main;
    //    Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
    //    return viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1;
    //}

    public void SpawnEnemies(byte opacity,float speed,int health,float damage)
    {
        timeGost += Time.deltaTime;
        if (timeGost >= timeStep)
        {
            int i = Random.Range(0, EnemyBody.Length);
            GameObject enemy = Instantiate(EnemyBody[i], new Vector3(gameObject.transform.position.x + Random.Range(-30, 30), gameObject.transform.position.y + Random.Range(-30, 30), 1.8f), Quaternion.identity);
            enemy.GetComponentInChildren<SpriteRenderer>().color = new Color32(255, 255, 255, opacity);
            enemy.GetComponent<Forward>().speed *= speed;
            enemy.GetComponentInChildren<HealthPoint>().healthPoint = health;
            enemy.GetComponent<Attack>().damage *= damage;
           
            timeGost = 0;
        }
    }
}
