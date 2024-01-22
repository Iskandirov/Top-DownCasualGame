using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : SkillBaseMono
{
    public TowerWave damageObj;
    public BobmExplode bomb;
    Collider2D[] colliders;
    float agreTime = 3;
    Forward objEnemyMove;
    public float waterElement;
    public float fireElement;
    
    // Start is called before the first frame update
    void Start()
    {
        waterElement = PlayerManager.instance.Water;
        fireElement = PlayerManager.instance.Fire;

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
            TowerWave a = Instantiate(damageObj, transform.position, Quaternion.identity);
            a.waterElement = waterElement;
        }
    }
    private IEnumerator TimerLife()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        if (basa.stats[2].isTrigger)
        {
            BobmExplode a = Instantiate(bomb, transform.position, Quaternion.identity);
            a.fire = fireElement;
        }
        Destroy(gameObject);
    }
    private IEnumerator TimerAgre(Forward a)
    {
        yield return new WaitForSeconds(agreTime);
        if (a != null)
        {
            a.GetComponentInParent<Forward>().destination.target = PlayerManager.instance.transform;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (basa.stats[4].isTrigger)
        {
            colliders = Physics2D.OverlapCircleAll(transform.position, 16f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.isTrigger != true && collider.CompareTag("Enemy")
                    && collider.GetComponent<HealthPoint>() != null && collider.GetComponentInParent<Forward>() != null)
                {
                    collider.GetComponentInParent<Forward>().destination.target = transform;
                    objEnemyMove = collider.transform.root.GetComponent<Forward>();
                }
            }
        }
    }
}
