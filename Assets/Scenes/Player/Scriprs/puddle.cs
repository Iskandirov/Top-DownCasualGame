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
    HealthBossPart objHealthB;
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
                if (enemies[i] != null)
                {
                    if (enemies[i].GetComponent<HealthPoint>())
                    {
                        objHealth = enemies[i].GetComponent<HealthPoint>();
                        objHealth.ChangeToKick();
                        objHealth.healthPoint -= damage;
                    }
                    else if (enemies[i].GetComponent<HealthBossPart>())
                    {
                        objHealthB = enemies[i].GetComponent<HealthBossPart>();
                        objHealthB.ChangeToKick();
                        objHealthB.healthPoint -= damage;
                    }
                }
            }
            damageTick = damageTickMax;
        }
    }
}
