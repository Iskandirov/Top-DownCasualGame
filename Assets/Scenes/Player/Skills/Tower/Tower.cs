using FSMC.Runtime;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : SkillBaseMono
{
    public TowerWave damageObj;
    public BobmExplode bomb;
    Collider2D[] colliders;
    float agreTime = 3;
    FSMC_Executer objEnemyMove;
    public float waterElement;
    public float fireElement;
    Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        waterElement = PlayerManager.instance.Water;
        fireElement = PlayerManager.instance.Fire;
        objTransform = transform;
        if (basa.stats[1].isTrigger)
        {
            basa.lifeTime += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.damageTickMax -= basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }

        StartCoroutine(TimerSpell());
        StartCoroutine(TimerLife());
        StartCoroutine(TimerAgre(objEnemyMove));
    }
    private IEnumerator TimerSpell()
    {
        while (true)
        {
            yield return new WaitForSeconds(basa.damageTickMax);
            TowerWave a = Instantiate(damageObj, objTransform.position, Quaternion.identity);
            a.waterElement = waterElement;
        }
    }
    private IEnumerator TimerLife()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        if (basa.stats[2].isTrigger)
        {
            BobmExplode a = Instantiate(bomb, objTransform.position, Quaternion.identity);
            a.fire = fireElement;
            a.damage *= player.GivePerkStatValue(Stats.ExplosionDamage) / 100;
        }
        Destroy(gameObject);
    }
    private IEnumerator TimerAgre(FSMC_Executer a)
    {
        yield return new WaitForSeconds(agreTime);
        if (a != null)
        {
            a.GetComponent<AIDestinationSetter>().target = PlayerManager.instance.objTransform;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (basa.stats[4].isTrigger)
        {
            colliders = Physics2D.OverlapCircleAll(objTransform.position, 16f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.isTrigger != true && collider.CompareTag("Enemy")
                    && collider.GetComponent<FSMC_Executer>() != null)
                {
                    collider.GetComponent<FSMC_Executer>().GetComponent<AIDestinationSetter>().target = objTransform;
                    objEnemyMove = collider.transform.root.GetComponent<FSMC_Executer>();
                }
            }
        }
    }
}
