using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    public float cold;
    public float damageTick;
    public float damageTickMax;
    void Start()
    {
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    private void Update()
    {
        damageTick -= Time.deltaTime;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        IceWallDeal(collision);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        IceWallDeal(collision);
    }
    void IceWallDeal(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (collision.GetComponentInParent<Forward>())
            {
                StartCoroutine(collision.GetComponentInParent<Forward>().SlowEnemy(1f, 0.1f));
                if (damageTick <= 0)
                {
                    collision.GetComponent<HealthPoint>().TakeDamage((damage * cold) / collision.GetComponent<HealthPoint>().Fire);
                    damageTick = damageTickMax;
                }
            }
        }
    }
}
