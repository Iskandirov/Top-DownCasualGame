using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float speed = 10f;
    public Rigidbody2D rb;


    public float forceAmount = 30f; // Сила відштовхування
    public bool isPiers;
    private void FixedUpdate()
    {
        Invoke("DestroyBullet", 1f);

    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || collision.GetComponent<HealthBossPart>())
            {
                collision.GetComponent<HealthBossPart>().healthPoint -= damage;
                collision.GetComponent<HealthBossPart>().ChangeToKick();
                if (!isPiers)
                {
                    Destroy(gameObject);
                }
            }
            else if (!collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || !collision.GetComponent<HealthBossPart>())
            {
                collision.GetComponent<HealthPoint>().healthPoint -= damage;
                collision.GetComponent<HealthPoint>().ChangeToKick();
                collision.transform.root.GetComponent<Forward>().isShooted = true;
                collision.transform.root.GetComponent<Forward>().Body.AddForce(-(transform.position - collision.transform.position) * forceAmount, ForceMode2D.Impulse);
                if (!isPiers)
                {
                    Destroy(gameObject);
                }
            }
            
        }
        else if (collision.CompareTag("Barrel"))
        {
            collision.gameObject.GetComponent<ObjectHealth>().health -= 1;
            if (!isPiers)
            {
                Destroy(gameObject);
            }
        }
        else if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
   
}
