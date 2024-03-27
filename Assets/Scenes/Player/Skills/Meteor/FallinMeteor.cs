using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FallinMeteor : SkillBaseMono
{
    [SerializeField] Meteor meteor;
    [SerializeField] List<EnemyState> enemiesInDanger;
    public EnemyController enemy;
    private void Start()
    {
        enemy = EnemyController.instance;
        if (basa.stats[2].isTrigger)
        {
            basa.countObjects += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
            StartCoroutine(WaitToAnotherObject(2, basa.spawnDelay));
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
    public void Fall()
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
            enemy.TakeDamage(creature, damage);
            GameManager.Instance.FindStatName("meteorDamage", damage);
            if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 3 && s.isActive == true) != null)
            {
                DailyQuests.instance.UpdateValue(3, damage, false);
            }
        }
        Meteor a = Instantiate(meteor, transform.position, Quaternion.identity);
        a.basa = basa;
        CineMachineCameraShake.instance.Shake(10, .1f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemiesInDanger.Add(collision.GetComponent<EnemyState>());
        }
    } 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemiesInDanger.Remove(collision.GetComponent<EnemyState>());
        }
    }
}
