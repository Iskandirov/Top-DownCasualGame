using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : SkillBaseMono
{
    public float damageTick;
    public bool isThree;
    public bool isFour;
    public bool isFive;
    public float fireDirt;
    
    // Start is called before the first frame update
    void Start()
    {

        //basa = SetToSkillID(gameObject);
        transform.localScale = new Vector3(transform.localScale.x + basa.radius, transform.localScale.y + basa.radius);

        damageTick = basa.damageTickMax;
        fireDirt = PlayerManager.instance.Dirt + PlayerManager.instance.Fire - 1;
        if (isThree)
        {
            Meteor b = Instantiate(this, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
            b.basa.damage = basa.damage;
            b.isFour = isFour;
            b.fireDirt = PlayerManager.instance.Dirt + PlayerManager.instance.Fire - 1;
            Meteor c = Instantiate(this, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
            c.basa.damage = basa.damage;
            c.isFour = isFour;
            c.fireDirt = PlayerManager.instance.Dirt + PlayerManager.instance.Fire - 1;
            if (isFive)
            {
                for (int i = 0; i < 5; i++)
                {
                    Meteor x = Instantiate(this, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
                    x.basa.damage = basa.damage;
                    x.isFour = isFour;
                    x.fireDirt = PlayerManager.instance.Dirt + PlayerManager.instance.Fire - 1;
                }
            }
        }
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        Destroy(gameObject);
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
                if (collision.GetComponentInParent<ElementActiveDebuff>() != null && !collision.GetComponentInParent<ElementActiveDebuff>().IsActive("isFire", true))
                {
                    collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isFire", true, true);
                    collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isFire", true, false);
                }
                if (isFour)
                {
                    collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax * fireDirt / 1.5f;
                }
                collision.GetComponent<HealthPoint>().TakeDamage((basa.damage * fireDirt * collision.GetComponent<HealthPoint>().Water) / collision.GetComponent<HealthPoint>().Fire);
                GameManager.Instance.FindStatName("meteorDamage", (basa.damage * fireDirt * collision.GetComponent<HealthPoint>().Water) / collision.GetComponent<HealthPoint>().Fire);
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
            if (isFour)
            {
                collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax;
            }
        }
    }
}
