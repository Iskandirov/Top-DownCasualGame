using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWave : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    public float burnDamage;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Move>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime-= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
        transform.position = player.transform.position;
        transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * 20, transform.localScale.y + Time.deltaTime * 20, transform.localScale.z + Time.deltaTime * 20);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || collision.GetComponent<HealthBossPart>())
            {
                collision.GetComponent<HealthBossPart>().healthPoint -= damage;
                collision.GetComponent<HealthBossPart>().ChangeToKick();
                if (burnDamage != 0)
                {
                    collision.GetComponent<HealthBossPart>().isBurn = true;
                    collision.GetComponent<HealthBossPart>().burnTime = 3;
                    collision.GetComponent<HealthBossPart>().burnDamage = burnDamage;
                }
            }
            else if (!collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || !collision.GetComponent<HealthBossPart>())
            {
                collision.GetComponent<HealthPoint>().healthPoint -= damage;
                collision.GetComponent<HealthPoint>().ChangeToKick();
                if (burnDamage != 0)
                {
                    collision.GetComponent<HealthPoint>().isBurn = true;
                    collision.GetComponent<HealthPoint>().burnTime = 3;
                    collision.GetComponent<HealthPoint>().burnDamage = burnDamage;
                }
            }
        }
    }
}
