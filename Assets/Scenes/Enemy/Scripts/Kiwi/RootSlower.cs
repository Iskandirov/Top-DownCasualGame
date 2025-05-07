using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSlower : MonoBehaviour
{
    public float damage;
    public float damageDelay;
    float damageDelayMax;
    public float lifeTime;
    PlayerManager player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        damageDelayMax = damageDelay;
        Invoke("DestroObj", lifeTime);
    }
    public void DestroObj()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        damageDelay -= Time.fixedDeltaTime;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield"))
        {
            if (damageDelay <= 0)
            {
                collision.GetComponent<Shield>().healthShield -= damage;
                FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage);
                damageDelay = damageDelayMax;
            }
        }
        else if (collision.CompareTag("Player") && !collision.isTrigger && !player.isInvincible)
        {
            if (damageDelay <= 0)
            {
                player.TakeDamage(damage);
                damageDelay = damageDelayMax;
                player.StartSlowPlayer(damageDelay * 0.9f, 0.9f);
            }
        }
    }
}
