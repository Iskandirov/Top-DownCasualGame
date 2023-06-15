using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float lifeTime;
    public float damageTick;
    public float damageTickMax;
    public float damage;
    public bool isFour;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        damageTick -= Time.deltaTime;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (damageTick <= 0)
        {
            if (collision.CompareTag("Enemy"))
            {
                if (collision.GetComponentInParent<ElementalBoss_Destroy>() || collision.GetComponent<HealthBossPart>())
                {
                    collision.GetComponent<HealthBossPart>().healthPoint -= damage;
                    collision.GetComponent<HealthBossPart>().ChangeToKick();
                    if (isFour)
                    {
                        collision.transform.root.GetComponent<Forward>().speed = collision.transform.root.GetComponent<Forward>().speed / 1.5f;
                    }
                }
                else if (!collision.GetComponentInParent<ElementalBoss_Destroy>() || !collision.GetComponent<HealthBossPart>())
                {
                    collision.GetComponent<HealthPoint>().healthPoint -= damage;
                    collision.GetComponent<HealthPoint>().ChangeToKick();
                    if (isFour)
                    {
                        collision.GetComponentInParent<Forward>().speed = collision.GetComponentInParent<Forward>().speedMax / 1.5f;
                    }
                }
                damageTick = damageTickMax;
            }
            else if (collision.CompareTag("Barrel"))
            {
                collision.GetComponent<ObjectHealth>().health -= 1;
                damageTick = damageTickMax;

            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponentInParent<ElementalBoss_Destroy>() || collision.GetComponent<HealthBossPart>())
            {
                if (isFour)
                {
                    collision.transform.root.GetComponent<Forward>().speed = collision.transform.root.GetComponent<Forward>().speed;
                }
            }
            else if (!collision.GetComponentInParent<ElementalBoss_Destroy>() || !collision.GetComponent<HealthBossPart>())
            {
                if (isFour)
                {
                    collision.GetComponentInParent<Forward>().speed = collision.GetComponentInParent<Forward>().speedMax;
                }
            }
        }
    }
}
