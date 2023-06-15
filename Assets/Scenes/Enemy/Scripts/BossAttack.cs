using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float spawnInterval = 2f;

    private float angle = 0f;
    public float speed = 5f;

    public void Start()
    {
        StartCoroutine(TimerCoroutine());
    }
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            MyMethod();
        }
    }
    private void MyMethod()
    {
        // Випустити об'єкти з відповідними напрямками
        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 45f) * Mathf.Deg2Rad), Mathf.Sin((angle + 45f) * Mathf.Deg2Rad));
        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 135f) * Mathf.Deg2Rad), Mathf.Sin((angle + 135f) * Mathf.Deg2Rad));
        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 225f) * Mathf.Deg2Rad), Mathf.Sin((angle + 225f) * Mathf.Deg2Rad));
        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos((angle + 325f) * Mathf.Deg2Rad), Mathf.Sin((angle + 325f) * Mathf.Deg2Rad));

        angle += 45f;
    }
}