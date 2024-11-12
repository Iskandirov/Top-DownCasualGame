using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float damage;
    public string[] targetPrefabNames;
    public Transform enemyBody;
    public Transform objTransform;
    Rigidbody2D rb;
    void Start()
    {
        objTransform = transform;
        rb = GetComponent<Rigidbody2D>();
    }
    public void DestroyBullet()
    {
        rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            collision.GetComponent<PlayerManager>().TakeDamage(damage);
            DestroyBullet();
        }
        else if (collision.CompareTag("Shield"))
        {
            collision.GetComponent<Shield>().healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
            DestroyBullet();
        }
    }
}
