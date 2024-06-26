using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Attack_Lightning : MonoBehaviour
{
    public GameObject target; 
    public GameObject objectToSpawn; 
    public float spawnRadius = 5.0f; 
    public float spawnInterval = 3.0f;
    public float damage = 20;
    public float objectsCount = 50;
    Transform objTransform;
    private void Start()
    {
        objTransform = transform;
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
                            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
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
        for (int i = 0; i <= objectsCount; i++)
        {
            Vector3 spawnPosition = objTransform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.z = 0f;
            Instantiate(target, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(delay);
        }
    }
}
