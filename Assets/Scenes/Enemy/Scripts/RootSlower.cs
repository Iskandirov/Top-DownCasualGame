using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSlower : MonoBehaviour
{
    public float damage;
    public float slowdownEndTime;
    public float damageDelay;
    float damageDelayMax;
    public float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        damageDelayMax = damageDelay;
        Invoke("DestroObj", lifeTime);
    }
    public void DestroObj()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        damageDelay -= Time.deltaTime;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !collision.GetComponent<Move>().isUntouchible)
        {
            if (damageDelay <= 0 && !collision.CompareTag("Shield"))
            {
                collision.GetComponent<Health>().playerHealthPoint -= damage;
                FindObjectOfType<StatsCollector>().FindStatName("DamageTaken", damage);
                collision.GetComponent<Health>().playerHealthPointImg.fullFillImage.fillAmount -= damage / collision.GetComponent<Health>().playerHealthPointMax;
                collision.GetComponent<Health>().GetComponent<Animator>().SetBool("IsHit", true);
                damageDelay = damageDelayMax;
            }
            if (!collision.GetComponent<Move>().isSlowingDown)
            {
                collision.GetComponent<Move>().isSlowingDown = true;
                collision.GetComponent<Move>().slowdownEndTime = slowdownEndTime;
                collision.GetComponent<Move>().slowPercent = 0.5f;
            }

        }
        else if (collision.CompareTag("Shield"))
        {
            if (damageDelay <= 0)
            {
                collision.GetComponent<Shield>().healthShield -= damage;
                FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage);
                damageDelay = damageDelayMax;
            }
        }
    }
}
