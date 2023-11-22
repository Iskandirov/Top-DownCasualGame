using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    public float tick;
    public float tickMax;
    public GameObject player;
    public float Steam;
    public float radius;
    public float addToAndle;
    public SpriteRenderer img;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Health>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;  // Закріплюємо координату Z на площині
        // Визначаємо відстань від гравця до позиції курсора
        Vector3 direction = mousePosition - player.transform.position;
        direction = direction.normalized * radius;

        // Повертаємо коло в напрямку курсора
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + addToAndle, Vector3.forward);

        tick -= Time.deltaTime;
        transform.position = new Vector2(player.transform.position.x, player.transform.position.y);
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HealthPoint objHealt = collision.GetComponent<HealthPoint>();
            
            if (collision.GetComponentInParent<ElementActiveDebuff>() != null && !collision.GetComponentInParent<ElementActiveDebuff>().IsActive("isSteam", true))
            {
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, false);
            }
            objHealt.healthPoint -= (damage * Steam * objHealt.Steam) / objHealt.Cold;
            FindObjectOfType<StatsCollector>().FindStatName("beamDamage", (damage * Steam * objHealt.Steam) / objHealt.Cold);
            objHealt.ChangeToKick();
        }
        else if (collision.CompareTag("Barrel") && collision != null)
        {
            collision.GetComponent<ObjectHealth>().health -= 1;
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (tick <= 0)
            {
                tick = tickMax;
                HealthPoint objHealt = collision.GetComponent<HealthPoint>();
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, false);
                objHealt.healthPoint -= (damage * Steam * objHealt.Steam) / objHealt.Cold;
                objHealt.ChangeToKick();
            }
        }
        else if (collision.CompareTag("Barrel") && collision != null)
        {
            collision.GetComponent<ObjectHealth>().health -= 1;
        }
    }
}
