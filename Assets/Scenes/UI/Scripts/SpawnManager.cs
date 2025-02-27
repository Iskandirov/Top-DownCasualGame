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
        while (true)
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
    public Vector3 GetRandomPositionInsideCollider()
    {
        // Отримати мінімальні та максимальні координати spawnArea
        Vector2 min = spawnMapBoundStatic.bounds.min;
        Vector2 max = spawnMapBoundStatic.bounds.max;

        // Створити об'єкт в випадковій точці всередині spawnArea
        System.Random a = new System.Random();
        do
        {
            randomPosition = new Vector3(
             a.Next((int)min.x, (int)max.x),
            a.Next((int)min.y, (int)max.y),
            0);
        }
        while (GetSpawnPositionsInAIPath(radius, randomPosition) == null);

        return randomPosition;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(randomPosition, radius);
    }
    public static Vector3 GetSpawnPositionsInAIPath(float radius, Vector3 center) // Додано параметр center
    {
        Vector3 availablePositions = new Vector3();

        if (AstarPath.active == null || AstarPath.active.data == null || AstarPath.active.data.gridGraph == null || AstarPath.active.data.gridGraph.nodes == null)
        {
            Debug.LogError("A* Pathfinding graph is not initialized or has no nodes!");
            return availablePositions;
        }

        var graph = AstarPath.active.data.gridGraph;
        var nodes = graph.nodes;

        foreach (var node in nodes)
        {
            if (node.Walkable)
            {
                // Обчислюємо відстань між центром вузла та центром області пошуку
                float distance = Vector2.Distance((Vector3)node.position, center);

                if (distance <= radius)
                {
                    availablePositions = new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid);
                }
            }
        }

        return availablePositions;
    }
}
