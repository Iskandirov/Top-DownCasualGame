using System;
using System.Collections;
using System.Collections.Generic;
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
            //SpawnObjectInMapBounds(barrel);
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
