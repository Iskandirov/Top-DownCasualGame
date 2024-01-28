using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FireWave : SkillBaseMono
{
    public float burnDamage;
    public float fireElement;
    PlayerManager player;
    Transform objTransform;
    EnemyController enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = EnemyController.instance;
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
        objTransform.position = player.objTransform.position;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            ElementActiveDebuff debuff = collision.GetComponent<ElementActiveDebuff>();
            EnemyState health = collision.GetComponent<EnemyState>();

            if (debuff != null)
            {
                debuff.StartCoroutine(debuff.EffectTime(Elements.status.Fire, 5));
            }
            enemy.TakeDamage(health, (basa.damage * debuff.elements.CurrentStatusValue(Elements.status.Water)) 
                / debuff.elements.CurrentStatusValue(Elements.status.Fire));
            GameManager.Instance.FindStatName("fireWaveDamage", (basa.damage * debuff.elements.CurrentStatusValue(Elements.status.Water)) 
                / debuff.elements.CurrentStatusValue(Elements.status.Fire));

            if (burnDamage != 0 && collision != null)
            {
                health.SetBurn();
                EnemyController.instance.Burn(health,3, 0.2f, burnDamage);
            }
        }
    }
}
