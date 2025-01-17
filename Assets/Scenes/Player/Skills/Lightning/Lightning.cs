using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lightning : SkillBaseMono
{
    public List<Collider2D> enemies;
    public List<Collider2D> enemiesToShoot;
    public Collider2D enemyToShoot;
    public float stunTime;
    public string target;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;

        //basa = SetToSkillID(gameObject);
      
        if (basa.stats[1].isTrigger)
        {
            basa.countObjects += (int)basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.damage *= basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            stunTime = basa.stats[2].value;
            //basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            basa.stepMax -= basa.stats[4].value;
            basa.skill.skillCD -= StabilizateCurrentReload(basa.skill.skillCD, basa.stats[2].value);
            basa.stats[4].isTrigger = false;
        }
        enemies = Physics2D.OverlapCircleAll(transform.position, basa.radius).ToList();
        if (enemies != null && enemies.Count > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.isTrigger != true && enemy.CompareTag(target))
                {
                    enemiesToShoot.Add(enemy);
                }
            }
            if (enemiesToShoot != null && enemiesToShoot.Count > 0)
            {
                enemyToShoot = enemiesToShoot[Random.Range(0, enemiesToShoot.Count - 1)];
                FSMC_Executer enemy = enemyToShoot.GetComponent<FSMC_Executer>();
                ElementActiveDebuff debuff = enemy.GetComponent<ElementActiveDebuff>();
                debuff.StartCoroutine(debuff.EffectTime(Elements.status.Electricity, 5));
                if (enemy != null && basa.stats[3].isTrigger)
                {
                    enemy.SetFloat("Stun Time", stunTime);
                }

                transform.position = enemyToShoot.transform.position;
                enemy.TakeDamage(basa.damage * player.Electricity / debuff.elements.CurrentStatusValue(Elements.status.Electricity));
                GameManager.Instance.FindStatName("lightDamage", basa.damage * player.Electricity
                    / debuff.elements.CurrentStatusValue(Elements.status.Electricity));
                CineMachineCameraShake.instance.Shake(10, .1f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
