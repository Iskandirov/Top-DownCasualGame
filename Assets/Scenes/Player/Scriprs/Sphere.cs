using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public float circleRadius;
    public float speed;
    public float angle;
    public float damage;
    private void FixedUpdate()
    {
        transform.position = PlayerManager.instance.transform.position + new Vector3(Mathf.Cos(Time.time * speed + angle) * circleRadius, Mathf.Sin(Time.time * speed + angle) * circleRadius, 0f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {

            collision.GetComponent<HealthPoint>().TakeDamage(damage);
            FindObjectOfType<SphereAround>().countSphere--;
            Destroy(gameObject);

        }
        else if (collision.CompareTag("Barrel"))
        {
            collision.gameObject.GetComponent<ObjectHealth>().health -= 1;
            FindObjectOfType<SphereAround>().countSphere--;
            Destroy(gameObject);
        }
    }
}