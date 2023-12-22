using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Attack_Lightning : MonoBehaviour
{
    public GameObject target; // ������ ��'���� ��� ������
    public GameObject objectToSpawn; // ������ ��'���� ��� ������
    public float spawnRadius = 5.0f;  // ����� ����������� ������
    public float spawnInterval = 3.0f; // �������� �� ��������
    public float damage = 20;
    private void Start()
    {
        // ��������� ��������, ��� ���� �������� ��'����
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            StartCoroutine(SpawnObjectsWithDelay(0.1f));
            // ������ �������� 3 ������
            yield return new WaitForSeconds(spawnInterval);

            // ������� �� ��'����, �� ���� ������� �����, � �������� ��� �� ������� ����
            GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Lightning");
            foreach (var obj in spawnedObjects)
            {
                bool playerInZone = Physics2D.OverlapCircle(obj.transform.position, 1, LayerMask.GetMask("Player"));
                Collider2D ShieldIsActive = Physics2D.OverlapCircle(obj.transform.position, 1, LayerMask.GetMask("PlayerIgnore"));
                if (playerInZone)
                {
                    if (ShieldIsActive != null)
                    {
                        if (ShieldIsActive.CompareTag("Shield"))
                        {
                            ShieldIsActive.GetComponent<Shield>().healthShield -= damage;
                            FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage);
                        }
                    }
                    else if(!PlayerManager.instance.isInvincible)
                    {
                        PlayerManager.instance.TakeDamage(damage);
                    }
                   
                }

                Vector3 spawnPosition = obj.transform.position;
                Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                Destroy(obj);
            }
        }
    }
    IEnumerator SpawnObjectsWithDelay(float delay)
    {
        for (int i = 0; i <= 50; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            Instantiate(target, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(delay);
        }
    }
}
