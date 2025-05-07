using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public enum GrapeType { Normal, Sticky, Explosive }
    public GrapeType typeOfBullet;

    [Header("General Settings")]
    public float speed = 10f;
    public float lifetime = 5f;

    [Header("Sticky Grape Settings")]
    public float slowDuration = 3f;
    public float slowAmount = 0.5f; // 50% від нормальної швидкості

    [Header("Explosive Grape Settings")]
    public float explosionRadius = 3f;
    public float explosionDelay = 1f;
    public int damage = 20;

    [Header("Circle Movement Settings")]
    public float circleRadius = 2f;  // Радіус кола, по якому будуть рухатися кулі
    public float angularSpeed = 90f; // Швидкість обертання навколо боса (градуси на секунду)

    public Transform bossTransform;  // Змінна для трансформації боса
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            playerManager.TakeDamage(10);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
