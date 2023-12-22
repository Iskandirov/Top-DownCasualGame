using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Attack_Lightning : MonoBehaviour
{
    public GameObject target; // Префаб об'єкта для спавну
    public GameObject objectToSpawn; // Префаб об'єкта для спавну
    public float spawnRadius = 5.0f;  // Радіус випадкового спавну
    public float spawnInterval = 3.0f; // Інтервал між спавнами
    public float damage = 20;
    private void Start()
    {
        // Запускаємо корутину, яка буде спавнити об'єкти
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            StartCoroutine(SpawnObjectsWithDelay(0.1f));
            // Чекаємо протягом 3 секунд
            yield return new WaitForSeconds(spawnInterval);

            // Знищуємо всі об'єкти, які були спавнуті раніше, і спавнуємо нові на їхньому місці
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
