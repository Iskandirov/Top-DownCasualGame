using FSMC.Runtime;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EnemyPool
{
    public GameObject enemyObj;
    public List<GameObject> enemyPool;
    public int poolSize;
}
public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] public List<FSMC_Executer> enemies { get; private set; }
    [field: SerializeField] public List<EnemyPool> enemiesPool { get; private set; }
    [field: SerializeField] public float enemySpawnInterval { get; private set; }
    [field: SerializeField] public Collider2D spawnMapBound { get; private set; }
    [field: SerializeField] public bool stopSpawn { get; private set; }
    [field: SerializeField] public Camera mainCamera { get; private set; }
    [field: SerializeField] public float spawnRadius { get; private set; }
    [field: SerializeField] public int enemycount { get; private set; }
    FSMC_Executer matchingEnemy;
    GameManager gameManager;
    public GameObject parent;
    public List<FSMC_Executer> children;
    public int timeToSpawnBoss;
    public bool isBossSpawned;
    public Timer timer;
    public GameObject boss;
    public static EnemySpawner instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        if (enemiesPool != null)
        {
            foreach (var enemyPool in enemiesPool)
            {
                enemyPool.enemyPool = InitializeObjectPool(enemies, enemyPool.enemyObj, enemyPool.poolSize, parent.transform);
            }
            StartCoroutine(SpawnEnemyRoutine(enemySpawnInterval));
        }
    }
    private void FixedUpdate()
    {
        if (timeToSpawnBoss <= timer.time && !isBossSpawned)
        {
            isBossSpawned = true;
            stopSpawn = true;
            children.Clear();
            DeleteChildren();
            GameObject a = Instantiate(boss, PlayerManager.instance.transform.position + new Vector3(0, 50f, 0), Quaternion.identity, parent.transform);
            children.Add(a.GetComponent<FSMC_Executer>());
        }
    }
    public void DeleteChildren()
    {
        int childCount = parent.transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = parent.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }
    static public bool GetSpawnPositionNotInAIPath(float radius, Vector2 pos)
    {
        // Ñòâîðèòè ñïèñîê äîñòóïíèõ ïîçèö³é
        List<Vector3> availablePositions = new List<Vector3>();

        // Îòðèìàòè âóçëè ñ³òêè
        var nodes = AstarPath.active.data.gridGraph.nodes;

        // Ïåðåáðàòè âñ³ âóçëè â ðàä³óñ³
        foreach (var node in nodes.Where(n => Mathf.Abs(n.position.x) > radius || Mathf.Abs(n.position.y) > radius))
        {
            if (node.Walkable)
            {
                availablePositions.Add(new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid));
                Debug.Log(new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid));
            }
        }

        return availablePositions.Contains(pos);
    }
    List<GameObject> InitializeObjectPool(List<FSMC_Executer> enemy, GameObject objectPrefab, int pool, Transform parent)
    {

        List<GameObject> objectPool = new List<GameObject>();
        for (int i = 0; i < pool; i++)
        {
            matchingEnemy = enemy.First(s => s.gameObject == objectPrefab);

            GameObject obj = Instantiate(objectPrefab, parent);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
            children.Add(obj.GetComponent<FSMC_Executer>());
        }
        return objectPool;
    }
    public Vector3 SpawnObjectInMapBounds()
    {
        // Îòðèìóºìî öåíòð êîëàéäåðà
        Vector2 colliderCenter = spawnMapBound.bounds.center;
        Vector2 randomPointInsideCollider = colliderCenter + UnityEngine.Random.insideUnitCircle * new Vector2(spawnMapBound.bounds.size.x * 0.6f, spawnMapBound.bounds.size.y * 0.5f);
        return randomPointInsideCollider;
    }
    GameObject GetFromPool(List<GameObject> objectPool)
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }
    private IEnumerator SpawnEnemyRoutine(float interval)
    {
        while (!stopSpawn)
        {
            for (int i = 0; i < enemiesPool.Count; i++)
            {
                for (int y = 0; y < enemiesPool[i].poolSize; y++)
                {
                    yield return new WaitForSeconds(interval);
                    int sizeOfPools = 0;
                    foreach (var size in enemiesPool)
                    {
                        sizeOfPools += size.poolSize;
                    }

                    int activeCount = 0;
                    foreach (Transform child in parent.transform)
                    {
                        if (child.gameObject.activeSelf)
                        {
                            activeCount++;
                        }
                    }
                    if (sizeOfPools > activeCount && !stopSpawn) 
                    {
                        matchingEnemy = enemies.FirstOrDefault(s => s.name == enemiesPool[i].enemyObj.name);
                        SpawnEnemies(enemiesPool[i].enemyPool, matchingEnemy.isSpawnOutsideCamera);
                    }
                    else
                    {
                        stopSpawn = true;
                        break;
                    }
                }
            }
        }
    }
    //private bool IsInsideCameraBounds(Vector3 position, bool needToBeOutside)
    //{
    //    Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
    //    if (needToBeOutside)
    //        return viewportPosition.x >= 0f && viewportPosition.x <= 1f && viewportPosition.y >= 0f && viewportPosition.y <= 1f;
    //    else
    //        return viewportPosition.x <= 0.2f && viewportPosition.x >= 0.8f && viewportPosition.y <= 0.2f && viewportPosition.y >= 0.8f;
    //}
    private bool IsInsideWallBounds(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
    //public Vector3 GetRandomSpawnPosition(Vector3 pos, bool needToBeOutside, float radius)
    //{
    //    Vector3 spawnPosition;
    //    do
    //    {
    //        float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
    //        Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * radius;
    //        spawnPosition = new Vector3(pos.x + spawnOffset.x, pos.y + spawnOffset.y, 0);
    //    } while (IsInsideCameraBounds(spawnPosition, needToBeOutside) || IsInsideWallBounds(spawnPosition));

    //    return spawnPosition;
    //}
    public Vector3 SpawnEnemyOutsideCamera()
    {
        Vector3 spawnPosition;
        do
        {
            // Визначаємо межі камери в світових координатах
            Vector3 cameraPos = mainCamera.transform.position;
            //float halfHeight = mainCamera.orthographicSize;
            //float halfWidth = halfHeight * mainCamera.aspect;

            // Генеруємо випадкову точку на одиничному колі
            System.Random a = new System.Random();
            float randomAngle = a.Next(0, 360);
            Vector3 randomDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0);

            // Обчислюємо точку спавну на заданій відстані від камери
            spawnPosition = cameraPos + randomDirection * spawnRadius;
        } while (IsInsideWallBounds(spawnPosition));
        return spawnPosition;
    }

    private void SpawnEnemies(List<GameObject> pool, bool spawnChoise)
    {
        GameObject enemy = GetFromPool(pool);

        string name = enemy.name;
        string newName = name.Replace("(Clone)", "");
        enemy.name = newName;

        IDChecker(enemy.name);

        switch (spawnChoise)
        {
            case false:
                enemy.transform.position = SpawnEnemyOutsideCamera();
                break;
            case true:
                enemy.transform.position = SpawnManager.GetRandomPositionInsideCollider();
                break;
        }
        enemycount++;
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
