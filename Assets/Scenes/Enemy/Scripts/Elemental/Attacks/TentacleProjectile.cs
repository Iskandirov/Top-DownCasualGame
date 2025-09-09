using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleProjectile : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float returnSpeed = 15f;
    public float lifeTime = 3f;

    private Transform boss;
    private Vector3 targetPos;
    private bool returning = false;
    private bool hasGrabbed = false;
    private Transform grabbedPlayer;

    public void Initialize(Transform bossTransform, Vector3 target)
    {
        boss = bossTransform;
        targetPos = target;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!returning)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                StartReturn();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, boss.position, returnSpeed * Time.deltaTime);

            if (grabbedPlayer != null)
                grabbedPlayer.position = transform.position;

            if (Vector3.Distance(transform.position, boss.position) < 0.5f)
                Destroy(gameObject); // Рука "зникла" біля босса
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!returning && col.CompareTag("Player"))
        {
            hasGrabbed = true;
            grabbedPlayer = col.transform;
            StartReturn();
        }
    }

    private void StartReturn()
    {
        returning = true;
    }
}
