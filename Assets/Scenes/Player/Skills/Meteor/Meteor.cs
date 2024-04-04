using System.Collections;
using UnityEngine;

public class Meteor : SkillBaseMono
{
    public float damageTick;
    public bool isFive;
    public float fireDirt;

    public EnemyController enemy;
    private void Start()
    {
        enemy = EnemyController.instance;
        player = PlayerManager.instance;
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.lifeTime += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
       
        //basa = SetToSkillID(gameObject);
        transform.localScale = new Vector3(transform.localScale.x + basa.radius, transform.localScale.y + basa.radius);

        damageTick = basa.damageTickMax;
        fireDirt = PlayerManager.instance.Dirt + PlayerManager.instance.Fire - 1;

        CoroutineToDestroy(gameObject,basa.lifeTime);
    }
   
   
    // Update is called once per frame
    void FixedUpdate()
    {
        damageTick -= Time.fixedDeltaTime;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (damageTick <= 0)
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyState health = collision.GetComponent<EnemyState>();

                ElementActiveDebuff debuff = collision.GetComponent<ElementActiveDebuff>();
                if (debuff != null)
                {
                    debuff.StartCoroutine(debuff.EffectTime(Elements.status.Fire, 5));
                }
                if (basa.stats[3].isTrigger)
                {
                    EnemyController.instance.SlowEnemy(health, 1f, fireDirt / 1.5f);
                }
                float damage = (basa.damage * fireDirt * debuff.elements.CurrentStatusValue(Elements.status.Water))
                    / debuff.elements.CurrentStatusValue(Elements.status.Fire);
                enemy.TakeDamage(health, damage);
                GameManager.Instance.FindStatName("meteorDamage", damage);
                DailyQuests.instance.UpdateValue(3, damage, false);
                damageTick = basa.damageTickMax;
            }
            else if (collision.CompareTag("Barrel") && collision != null)
            {
                collision.GetComponent<ObjectHealth>().TakeDamage();
                damageTick = basa.damageTickMax;

            }
        }
    }
    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        if (basa.stats[3].isTrigger)
    //        {
    //            collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax;
    //        }
    //    }
    //}
}
