using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puddle : MonoBehaviour
{
    public float lifeTime;
    public float damageTick;
    public float damageTickMax;

    public Collider2D[] enemies;

    public float damage;

    public Sprite[] lights;
    public float radius;

    HealthPoint objHealth;
    ElementActiveDebuff objElement;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerSpell());
        StartCoroutine(CastSpell());
    }
    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    private IEnumerator CastSpell()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageTick);
            enemies = Physics2D.OverlapCircleAll(transform.position, radius);

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null && enemies[i].CompareTag("Enemy") && enemies[i].GetComponentInParent<ElementActiveDebuff>() != null && enemies[i].GetComponentInParent<ElementActiveDebuff>() != null)
                {
                    objHealth = enemies[i].GetComponent<HealthPoint>();
                    objElement = enemies[i].GetComponentInParent<ElementActiveDebuff>();
                   
                    if (!objElement.IsActive("isWater", true))
                    {
                        objElement.SetBool("isWater", true, true);
                        objElement.SetBool("isWater", true, false);

                    }
                    if (!objElement.IsActive("isDirt", true))
                    {
                        objElement.SetBool("isDirt", true, true);
                        objElement.SetBool("isDirt", true, false);
                    }
                    objHealth.healthPoint -= (damage * objHealth.Electricity) / objHealth.Water * objHealth.Dirt;
                    GameManager.Instance.FindStatName("puddleDamage", (damage * objHealth.Electricity) / objHealth.Water * objHealth.Dirt);
                }
            }
            damageTick = damageTickMax;
        }
    }
}
