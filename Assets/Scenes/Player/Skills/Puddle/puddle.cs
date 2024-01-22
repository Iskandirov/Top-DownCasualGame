using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class puddle : SkillBaseMono
{
    public Collider2D[] enemies;

    HealthPoint objHealth;
    ElementActiveDebuff objElement;
    PlayerManager player;
    Vector3 halfExtents;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        //Gizmos.DrawWireCube(transform.position, new Vector3((transform.localScale.x + basa.radius) * 4f, (transform.localScale.y + basa.radius), transform.localScale.z));
        //Gizmos.DrawWireCube(transform.position, 2 * halfExtents);
    }
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        if (basa.stats[1].isTrigger)
        {
            basa.radius += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.damage *= basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.countObjects += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
            StartCoroutine(WaitToAnotherObject(2, basa.spawnDelay));
        }
        if (basa.stats[4].isTrigger)
        {
            basa.damageTickMax -= basa.stats[4].value;
            basa.stats[4].isTrigger = false;
        }

        transform.localScale = new Vector2((transform.localScale.x + basa.radius) * player.Dirt, (transform.localScale.y + basa.radius) * player.Dirt);
        halfExtents = new Vector3(transform.localScale.x * 4f, transform.localScale.y, transform.localScale.z);


        StartCoroutine(CastSpell());
        CoroutineToDestroy(gameObject, basa.lifeTime);
    }
    private IEnumerator WaitToAnotherObject(int count, float delay)
    {
        for (int i = 0; i < count - 1; i++)
        {
            yield return new WaitForSeconds(delay);
            GameObject a = Instantiate(gameObject, new Vector3(player.transform.position.x + Random.Range(-20, 20), player.transform.position.y + Random.Range(-20, 20), 1.9f), Quaternion.identity);
            a.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    private IEnumerator CastSpell()
    {
        while (true)
        {
            enemies = Physics2D.OverlapBoxAll(transform.position, halfExtents, 0f);

            foreach(var enemy in enemies)
            {
                if (enemy != null && enemy.CompareTag("Enemy") && enemy.GetComponentInParent<ElementActiveDebuff>() != null)
                {
                    objHealth = enemy.GetComponent<HealthPoint>();
                    objElement = enemy.GetComponentInParent<ElementActiveDebuff>();

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
                    objHealth.healthPoint -= (basa.damage / objHealth.Electricity) * player.Water / objHealth.Water * objHealth.Dirt;
                    GameManager.Instance.FindStatName("puddleDamage", (basa.damage / objHealth.Electricity) * player.Water / objHealth.Water * objHealth.Dirt);
                }
            }

            yield return new WaitForSeconds(basa.damageTickMax);
        }
    }
}
