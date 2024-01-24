using System.Collections;
using UnityEngine;

public class Meteor : SkillBaseMono
{
    public float damageTick;
    public bool isFive;
    public float fireDirt;
    
    // Start is called before the first frame update
    void Start()
    {
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.countObjects += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
            StartCoroutine(WaitToAnotherObject(2, basa.spawnDelay));
        }
        if (basa.stats[3].isTrigger)
        {
            basa.lifeTime += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            basa.countObjects += basa.stats[4].value;
            basa.stats[4].isTrigger = false;
            StartCoroutine(WaitToAnotherObject(5,basa.spawnDelay));
        }
        //basa = SetToSkillID(gameObject);
        transform.localScale = new Vector3(transform.localScale.x + basa.radius, transform.localScale.y + basa.radius);

        damageTick = basa.damageTickMax;
        fireDirt = PlayerManager.instance.Dirt + PlayerManager.instance.Fire - 1;

        CoroutineToDestroy(gameObject,basa.lifeTime);
    }
   
    private IEnumerator WaitToAnotherObject(int count, float delay)
    {
        for (int i = 0; i < count - 1; i++)
        {
            yield return new WaitForSeconds(delay);
            Meteor b = Instantiate(this, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
            b.basa.damage = basa.damage;
        }
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
                ElementActiveDebuff element = collision.GetComponentInParent<ElementActiveDebuff>();
                HealthPoint health = collision.GetComponent<HealthPoint>();

                if (element != null && !element.IsActive("isFire", true))
                {
                    element.SetBool("isFire", true, true);
                    element.SetBool("isFire", true, false);
                }
                if (basa.stats[3].isTrigger)
                {
                    collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax * fireDirt / 1.5f;
                }
                health.TakeDamage((basa.damage * fireDirt * health.Water) / health.Fire);
                GameManager.Instance.FindStatName("meteorDamage", (basa.damage * fireDirt * health.Water) / health.Fire);
                damageTick = basa.damageTickMax;
            }
            else if (collision.CompareTag("Barrel") && collision != null)
            {
                collision.GetComponent<ObjectHealth>().health -= 1;
                damageTick = basa.damageTickMax;

            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (basa.stats[3].isTrigger)
            {
                collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax;
            }
        }
    }
}
