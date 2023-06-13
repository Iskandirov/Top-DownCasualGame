using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float speed = 5f;
    public float spawnInterval = 2f;

    private float timeSinceLastSpawn = 0f;
    private float angle = 0f;

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            timeSinceLastSpawn = 0f;

            // Випустити об'єкти з відповідними напрямками
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 45f) * Mathf.Deg2Rad), Mathf.Sin((angle + 45f) * Mathf.Deg2Rad));
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 135f) * Mathf.Deg2Rad), Mathf.Sin((angle + 135f) * Mathf.Deg2Rad));
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 225f) * Mathf.Deg2Rad), Mathf.Sin((angle + 225f) * Mathf.Deg2Rad));
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 325f) * Mathf.Deg2Rad), Mathf.Sin((angle + 325f) * Mathf.Deg2Rad));

            angle += 45f;
        }
    }
}