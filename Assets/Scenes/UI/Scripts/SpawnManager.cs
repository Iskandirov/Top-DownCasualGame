using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(11)]
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager inst;

    [Header("Barrels")]
    public int barrelPoolSize = 5;
    public int explosionBarrelPoolSize = 5;
    public List<GameObject> barrelLootPool;
    public List<GameObject> barrelExplosionPool;
    public float barrelSpawnInterval = 20f;
    public GameObject[] barrelPrefab;

    [field: SerializeField] public static Collider2D spawnMapBoundStatic { get; private set; }
    [field: SerializeField] public Collider2D spawnMapBound { get; private set; }
    public float radius;
    Vector3 randomPosition;
    private void Awake()
    {
        spawnMapBoundStatic = spawnMapBound;
        inst ??= this;
    }
    // Start is called before the first frame update
    void Start()
    {
        barrelLootPool = InitializeObjectPool(barrelPrefab[0], barrelPoolSize, transform);
        barrelExplosionPool = InitializeObjectPool(barrelPrefab[1], explosionBarrelPoolSize, transform);

        StartCoroutine(SpawnRoutine(barrelLootPool, barrelPrefab[0], barrelSpawnInterval));
        StartCoroutine(SpawnRoutine(barrelExplosionPool, barrelPrefab[1], barrelSpawnInterval));

    }
    public void DestroyBarrels()
    {
        for (int i = 0; i < barrelLootPool.Count; i++)
        {
            Destroy(barrelLootPool[i]);
        }
        barrelLootPool.Clear();
    }
    List<GameObject> InitializeObjectPool(GameObject objectPrefab, int pool, Transform parent)
    {
        List<GameObject> objectPool = new List<GameObject>();
        for (int i = 0; i < pool; i++)
        {
            GameObject obj = Instantiate(objectPrefab, parent);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
        return objectPool;
    }
    GameObject GetFromPool(List<GameObject> objectPool, GameObject objectPrefab)
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject gameObject = Instantiate(objectPrefab);
        objectPool.Add(gameObject);
        return gameObject;
    }

    private IEnumerator SpawnRoutine(List<GameObject> objList, GameObject obj, float interval)
    {
        while (!EnemySpawner.instance.isBossSpawned)
        {
            yield return new WaitForSeconds(interval);
            GameObject barrel = GetFromPool(objList, obj);
            barrel.transform.position = GetRandomPositionInsideCollider();
            // Отримати координати та розмір нового об'єкта
            var bounds = barrel.GetComponentInChildren<Renderer>().bounds;
            var minX = bounds.min.x;
            var minZ = bounds.min.z;
            var maxX = bounds.max.x;
            var maxZ = bounds.max.z;

            var map = AstarPath.active.data.gridGraph.nodes;
            // Визначити область на карті, яка буде заблокована новим об'єктом
            foreach (var node in map)
            {
                if (node.XCoordinateInGrid >= minX && node.XCoordinateInGrid <= maxX && node.ZCoordinateInGrid >= minZ && node.ZCoordinateInGrid <= maxZ)
                {
                    node.Walkable = false;
                }
            }
            AstarPath.active.UpdateGraphs(bounds);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(randomPosition, radius);
    }
    public Vector3 GetRandomPositionInsideCollider()
    {
        // Отримати мінімальні та максимальні координати spawnArea
        Vector2 min = spawnMapBoundStatic.bounds.min;
        Vector2 max = spawnMapBoundStatic.bounds.max;

        int attempts = 0;
        int maxAttempts = 100; // щоб уникнути нескінченного циклу

        while (attempts < maxAttempts)
        {
            Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(min.x, max.x),
                UnityEngine.Random.Range(min.y, max.y),
                0);

            if (IsPositionWalkable(randomPosition))
            {
                return randomPosition;
            }

            attempts++;
        }

        Debug.LogWarning("Could not find walkable position after " + maxAttempts + " attempts.");
        return Vector3.zero; // fallback
    }

    private bool IsPositionWalkable(Vector3 position)
    {
        if (AstarPath.active == null || AstarPath.active.data == null)
        {
            Debug.LogError("A* Pathfinding is not initialized!");
            return false;
        }

        var graph = AstarPath.active.data.gridGraph;

        // Перевести світові координати в координати графа
        var node = graph.GetNearest(position).node;

        if (node == null)
        {
            Debug.LogWarning("No node found near position!");
            return false;
        }

        return node.Walkable;
    }
}
