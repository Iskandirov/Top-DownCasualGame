using System.Collections;
using System.Collections.Generic;
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
    public float speed = 5;
    public float angle = 5;
    public float lifeTime;
    public Transform objTransform;
    Rigidbody2D rb;
    void Start()
    {
        objTransform = transform;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAround && enemyBody != null)
        {
            // ¬изначаЇмо колову траЇктор≥ю навколо ц≥л≥
            Vector3 position = enemyBody.position + new Vector3(Mathf.Cos(Time.fixedTime * speed) * distance, Mathf.Sin(Time.fixedTime * speed) * distance, 0f);
            distance += 0.7f;

            objTransform.position = position;

            lifeTime = 3f;
        }
        else if (isRandome)
        {
            speed += 0.1f;
            lifeTime = 5f;
        }
        else
        {
            speed += 0.1f;
            lifeTime = 3f;
        }
    }
    public void DestroyBullet()
    {
        rb.velocity = Vector2.zero;
        speed = 2;
        distance = 5;
        isAround = false;
        isRandome = false;
        gameObject.SetActive(false);
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            PlayerManager.instance.TakeDamage(damage);
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
