using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BossBullet : MonoBehaviour
{
    public float damage;
    public string[] targetPrefabNames;
    public bool isAround;
    public bool isRandome;
    public Transform enemyBody;
    public float distance = 20f;
    public float speed = .5f;
    public float angle = 5;
    public Transform objTransform;
    Rigidbody2D rb;
    public bool state = false;
    public Vector3 startPos;
    public int index;
    public float angleOffset;
    void Start()
    {
        objTransform = transform;
        startPos = objTransform.localPosition;
        rb = GetComponent<Rigidbody2D>();
        index = transform.GetSiblingIndex();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!state && objTransform.localPosition != startPos)
        {
            objTransform.localPosition = startPos;
            distance = 5;
            speed = 1;
            angle = 50;
        }
        else if (state)
        {
            if (isAround && enemyBody != null)
            {
                angle += Time.fixedDeltaTime * angleOffset;
                Vector3 position = startPos + new Vector3(Mathf.Cos((angle + index)) * distance, Mathf.Sin((angle + index)) * distance, 0f);
                distance += 0.2f;

                objTransform.localPosition = position;

            }
            else if (isRandome)
            {
                Vector3 position = startPos + new Vector3(Mathf.Cos((Time.fixedDeltaTime + index) * speed) * distance, Mathf.Sin((Time.fixedDeltaTime + index) * speed) * distance, 0f);
                distance += 0.4f;
                objTransform.localPosition = position;
            }
        }

    }
    public void DestroyBullet()
    {
        rb.velocity = Vector2.zero;
        speed = 1;
        distance = 5;
        isAround = false;
        isRandome = false;
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && state)
        {
            PlayerManager.instance.TakeDamage(damage);
            DestroyBullet();
        }
        else if (collision.CompareTag("Shield") && state)
        {
            collision.GetComponent<Shield>().healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
            DestroyBullet();
        }
    }
}
