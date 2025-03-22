using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerTrail : MonoBehaviour
{
    public float hitDelay = 0.5f;
    public EnemySpawner enemies;
    public SkillBase basa;
    public List<Collider2D> enemiesColliders = new List<Collider2D>();
    private void Start()
    {
        enemies = FindAnyObjectByType<EnemySpawner>();
        StartCoroutine(DealDamage());
    }
    IEnumerator DealDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(hitDelay);
            if (enemiesColliders.Count > 0)
            {
                for (int i = 0; i < enemiesColliders.Count; i++)
                {
                    ElementActiveDebuff debuff = enemiesColliders[i].GetComponentInParent<ElementActiveDebuff>();
                    debuff.StartCoroutine(debuff.EffectTime(Elements.status.Grass, 5));
                    enemiesColliders[i].GetComponent<FSMC_Executer>().TakeDamage(basa.damage);
                    if (basa.stats[4].isTrigger)
                    {
                        if (enemiesColliders[i].GetComponent<FSMC_Executer>().health <= 0)
                        {
                            float heal = enemies.children.Find(s => s.name == enemiesColliders[i].GetComponent<FSMC_Executer>().name).healthMax * 0.1f;
                            if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 1 && s.isActive == true) != null)
                            {
                                DailyQuests.instance.UpdateValue(1, heal, false, true);
                            }
                            PlayerManager.instance.HealHealth(heal);
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (!enemiesColliders.Contains(collision))
            {
                enemiesColliders.Add(collision);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemiesColliders.Remove(collision);
        }
    }
}
