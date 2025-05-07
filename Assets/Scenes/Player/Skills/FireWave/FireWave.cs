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
                debuff.StartCoroutine(debuff.EffectTime(Elements.status.Fire, 5));
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
                StartCoroutine(Burn(health, 3,2, 0.2f));
            }
        }
    }
    IEnumerator Burn(FSMC_Executer enemy,float damage,float time,float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (time > 0)
            {
                time -= Time.fixedDeltaTime;
                delay -= Time.fixedDeltaTime;
                if (delay <= 0)
                {
                    enemy.TakeDamage(damage);
                }
            }
            else
            {
                //End Fire Anim
                //ChangeToNotKick();
            }
        }
    }
}
