using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lightning : SkillBaseMono
{
    public List<Collider2D> enemies;
    public List<Collider2D> enemiesToShoot;
    public Collider2D enemyToShoot;
    HealthPoint objHealth;
    Forward objMove;
    public float stunTime;
    PlayerManager player;
    

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
        if (basa.stats[4].isTrigger)
        {
            basa.stepMax -= basa.stats[4].value;
            basa.skill.skillCD -= StabilizateCurrentReload(basa.skill.skillCD, basa.stats[2].value);
            basa.stats[4].isTrigger = false;
        }
        enemies = Physics2D.OverlapCircleAll(gameObject.transform.position, basa.radius).ToList();
        if (enemies != null && enemies.Count > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.isTrigger != true && enemy.CompareTag("Enemy"))
                {
                    enemiesToShoot.Add(enemy);
                }
            }
            if (enemiesToShoot != null && enemiesToShoot.Count > 0)
            {
                enemyToShoot = enemiesToShoot[Random.Range(0, enemiesToShoot.Count - 1)];
                objHealth = enemyToShoot.GetComponent<HealthPoint>();
                objMove = enemyToShoot.GetComponentInParent<Forward>();
                if (objHealth.GetComponentInParent<ElementActiveDebuff>() != null && !objHealth.GetComponentInParent<ElementActiveDebuff>().IsActive("isElectricity", true))
                {
                    objHealth.GetComponentInParent<ElementActiveDebuff>().SetBool("isElectricity", true, true);
                    objHealth.GetComponentInParent<ElementActiveDebuff>().SetBool("isElectricity", true, false);
                }
                if (objMove != null && basa.stats[4].isTrigger)
                {
                    objMove.isStunned = true;
                    objMove.stunnTime = stunTime;
                }


                if (objHealth.IsBobs == true)
                {
                    enemyToShoot.GetComponentInParent<Animator>().SetBool("IsHit", true);
                }
                transform.position = enemyToShoot.transform.position;
                objHealth.healthPoint -= basa.damage * player.Electricity / objHealth.Electricity;
                GameManager.Instance.FindStatName("lightDamage", basa.damage * player.Electricity / objHealth.Electricity);
            }
            else 
            {
                Destroy(gameObject);
            }
        }
    }
        
}
