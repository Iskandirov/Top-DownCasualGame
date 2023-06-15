using System.Collections;
using UnityEngine;

public class SpawnBarrel : MonoBehaviour
{
    public GameObject objectPrefab; // ������ ��'����, ���� ������� ��������
    public Vector2 spawnAreaMin; // ̳������ ���������� ������
    public Vector2 spawnAreaMax; // ���������� ���������� ������
    public float spawnInterval = 1f; // �������� ������ � ��������

    //private bool isSpawning = false;

    public void Start()
    {
        StartCoroutine(SpawnRoutine());
    }
  
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnObject();
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
