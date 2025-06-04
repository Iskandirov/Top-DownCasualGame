using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireWave : SkillBaseMono
{
    public float burnDamage;
    Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objTransform = transform;
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.stepMax -= basa.stats[3].value;
            basa.skill.skillCD -= StabilizateCurrentReload(basa.skill.skillCD, basa.stats[3].value);

            basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            burnDamage = basa.stats[4].value;
        }
        basa.damage = basa.damage * player.Fire;
        objTransform.position = player.ShootPoint.transform.position;
    }
    public void IsNeedToDestroy()
    {
        if (!basa.stats[2].isTrigger)
        {
            Destroy(gameObject);
        }
    }
    void FixedUpdate()
    {
        //objTransform.position = player.ShootPoint.transform.position;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            ElementActiveDebuff debuff = collision.GetComponent<ElementActiveDebuff>();
            FSMC_Executer health = collision.GetComponent<FSMC_Executer>();

            if (debuff != null)
            {
                debuff.ApplyEffect(Elements.status.Fire, 5);
            }
            float damage = (basa.damage * debuff.elements.CurrentStatusValue(Elements.status.Water))
                / debuff.elements.CurrentStatusValue(Elements.status.Fire);

            health.TakeDamage(damage);
            GameManager.Instance.FindStatName("fireWaveDamage", damage);
            if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 3 && s.isActive == true) != null)
            {
                DailyQuests.instance.UpdateValue(3, damage, false, true);
            }

            if (burnDamage != 0 && collision != null)
            {
                burningEnemies.Add(health);

                if (!isBurning && !health.IsDead)
                {
                    StartCoroutine(BurnAll(burnDamage, 3, 0.2f));
                }
            }
        }
    }
    private List<FSMC_Executer> burningEnemies = new List<FSMC_Executer>();
    private bool isBurning = false;

    IEnumerator BurnAll(float damage, float time, float delay)
    {
        isBurning = true;

        while (burningEnemies.Count > 0 && time > 0)
        {
            yield return new WaitForSeconds(delay);

            for (int i = burningEnemies.Count - 1; i >= 0; i--)
            {
                FSMC_Executer enemy = burningEnemies[i];

                if (enemy == null || enemy.IsDead)
                {
                    burningEnemies.RemoveAt(i);
                    continue;
                }

                enemy.TakeDamage(damage);
            }

            time -= delay;
        }

        isBurning = false;
    }
}
