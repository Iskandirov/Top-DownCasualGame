using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBarrel : MonoBehaviour
{
    public GameObject objectPrefab; // ������ ��'����, ���� ������� ��������
    public Vector2 spawnAreaMin; // ̳������ ���������� ������
    public Vector2 spawnAreaMax; // ���������� ���������� ������
    public float spawnInterval = 1f; // �������� ������ � ��������

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
        // �������� �������� ���������� � ����� ������ ������ ������
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        // ��������� ��'��� � ������������� ������� � ���������� ���������
        Instantiate(objectPrefab, new Vector3(randomX, randomY, 0f), Quaternion.identity);
    }
}
