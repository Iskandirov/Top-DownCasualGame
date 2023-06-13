using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBarrel : MonoBehaviour
{
    public GameObject objectPrefab; // Префаб об'єкта, який потрібно спавнити
    public Vector2 spawnAreaMin; // Мінімальні координати спавну
    public Vector2 spawnAreaMax; // Максимальні координати спавну
    public float spawnInterval = 1f; // Інтервал спавну в секундах

    private bool isSpawning = false;

    public void FixedUpdate()
    {
        StartSpawning();
    }
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        if (isSpawning)
        {
            isSpawning = false;
            StopCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnObject();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnObject()
    {
        // Генеруємо випадкові координати в межах заданої області спавну
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        // Створюємо об'єкт з використанням префабу і випадкових координат
        Instantiate(objectPrefab, new Vector3(randomX, randomY, 0f), Quaternion.identity);
    }
}
