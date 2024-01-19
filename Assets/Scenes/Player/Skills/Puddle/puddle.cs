using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puddle : SkillBaseMono
{
    public float damageTick;

    public Collider2D[] enemies;

    public Sprite[] lights;

    HealthPoint objHealth;
    ElementActiveDebuff objElement;
    PlayerManager player;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, basa.radius);
    }
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        //basa = SetToSkillID(gameObject);
        damageTick = basa.damageTickMax;
        basa.damage = basa.damage * player.Water;
        basa.radius *= player.Dirt;
        gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x + basa.radius, gameObject.transform.localScale.y + basa.radius);
        StartCoroutine(TimerSpell());
        StartCoroutine(CastSpell());
    }
    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        Destroy(gameObject);
    }
    private IEnumerator CastSpell()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageTick);
            enemies = Physics2D.OverlapCircleAll(transform.position, basa.radius);

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
                    objHealth.healthPoint -= (basa.damage * objHealth.Electricity) / objHealth.Water * objHealth.Dirt;
                    GameManager.Instance.FindStatName("puddleDamage", (basa.damage * objHealth.Electricity) / objHealth.Water * objHealth.Dirt);
                }
            }
            damageTick = basa.damageTickMax;
        }
    }
}
