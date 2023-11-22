using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SniperTreeBullet : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !collision.GetComponent<Move>().isUntouchible)
        {
            collision.GetComponent<Health>().playerHealthPoint -= damage;
            FindObjectOfType<StatsCollector>().FindStatName("DamageTaken", damage);
            collision.GetComponent<Health>().playerHealthPointImg.fullFillImage.fillAmount -= damage / collision.GetComponent<Health>().playerHealthPointMax;
            collision.GetComponent<Animator>().SetBool("IsHit", true);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Shield"))
        {
            collision.GetComponent<Shield>().healthShield -= damage;
            FindObjectOfType<StatsCollector>().FindStatName("ShieldAbsorbedDamage", damage);
            Destroy(gameObject);
        }
    }
}
