using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FallinMeteor : SkillBaseMono
{
    [SerializeField] List<FSMC_Executer> enemiesInDanger;
    public float damageTick;
    private void Start()
    {
        player = PlayerManager.instance;
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.countObjects += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
            StartCoroutine(WaitToAnotherObject(2, basa.spawnDelay));
        }
        if (basa.stats[3].isTrigger)
        {
            basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            basa.countObjects += basa.stats[4].value;
            basa.stats[4].isTrigger = false;
            StartCoroutine(WaitToAnotherObject(5, basa.spawnDelay));
        }
    }
    private IEnumerator WaitToAnotherObject(int count, float delay)
    {
        for (int i = 0; i < count - 1; i++)
        {
            yield return new WaitForSeconds(delay);
            FallinMeteor b = Instantiate(this, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
            b.basa.damage = basa.damage;
        }
    }
    public void FixedUpdate()
    {
        damageTick -= Time.fixedDeltaTime;
        if (damageTick <= 0)
        {
            foreach (var creature in enemiesInDanger)
            {
                ElementActiveDebuff debuff = creature.GetComponent<ElementActiveDebuff>();
                if (debuff != null)
                {
                    debuff.StartCoroutine(debuff.EffectTime(Elements.status.Fire, 5));
                }
                float damage = (basa.damage * debuff.elements.CurrentStatusValue(Elements.status.Water))
                    / debuff.elements.CurrentStatusValue(Elements.status.Fire);
                creature.TakeDamage(damage);
                GameManager.Instance.FindStatName("meteorDamage", damage);
                if (basa.stats[3].isTrigger)
                {
                    creature.SetFloat("SlowPercent", basa.stats[3].value);
                    creature.SetFloat("SlowTime", 2f);
                }
                if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 3 && s.isActive == true) != null)
                {
                    DailyQuests.instance.UpdateValue(3, damage, false);
                }
            }
            damageTick = basa.damageTickMax;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemiesInDanger.Add(collision.GetComponent<FSMC_Executer>());
        }
    } 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemiesInDanger.Remove(collision.GetComponent<FSMC_Executer>());
        }
    }
}
