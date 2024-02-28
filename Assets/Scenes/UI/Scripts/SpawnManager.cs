using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(11)]
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager inst;

    [Header("Barrels")]
    public int barrelPoolSize = 5;
    public List<GameObject> barrelPool;
    public GameObject barrelPrefab; // Префаб об'єкта, який потрібно спавнити
    public float barrelSpawnInterval = 20f; // Інтервал спавну в секундах
    [field: SerializeField] public Collider2D spawnMapBound { get; private set; }

    private void Awake()
    {
        inst ??= this;
    }
    // Start is called before the first frame update
    void Start()
    {
        barrelPool = InitializeObjectPool(barrelPrefab, barrelPoolSize,transform);
       
        StartCoroutine(SpawnRoutine(barrelSpawnInterval));
       
    }
    List<GameObject> InitializeObjectPool(GameObject objectPrefab,int pool,Transform parent)
    {
        List<GameObject>  objectPool = new List<GameObject>();
        for (int i = 0; i < pool; i++)
        {
            GameObject obj = Instantiate(objectPrefab, parent);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
        return objectPool;
    }
    GameObject GetFromPool(List<GameObject> objectPool,GameObject objectPrefab)
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
    
    private IEnumerator SpawnRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            GameObject barrel = GetFromPool(barrelPool, barrelPrefab);
            SpawnObjectInMapBounds(barrel);
            // Отримати координати та розмір нового об'єкта
            var bounds = barrel.GetComponent<Renderer>().bounds;
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
    public void SpawnObjectInMapBounds(GameObject obj)
    {
        // Отримуємо центр колайдера
        Vector2 colliderCenter = spawnMapBound.bounds.center;
        Vector2 randomPointInsideCollider = colliderCenter + UnityEngine.Random.insideUnitCircle * new Vector2(spawnMapBound.bounds.size.x * 0.6f, spawnMapBound.bounds.size.y * 0.5f);
        obj.transform.position = randomPointInsideCollider;
    }


}
