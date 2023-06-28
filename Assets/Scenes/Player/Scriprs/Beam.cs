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
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Health>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        tick -= Time.deltaTime;
        transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 0.3f);
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
            objHealt.healthPoint -= (damage * Steam * objHealt.Steam) / objHealt.Cold;
            objHealt.ChangeToKick();
            collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true,true);
            collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true,false);
        }
        else if (collision.CompareTag("Barrel"))
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
                objHealt.healthPoint -= (damage * Steam * objHealt.Steam) / objHealt.Cold;
                objHealt.ChangeToKick();
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, false);
            }
        }
        else if (collision.CompareTag("Barrel"))
        {
            collision.GetComponent<ObjectHealth>().health -= 1;
        }
    }
}
