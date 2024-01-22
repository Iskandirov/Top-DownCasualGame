using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWave : SkillBaseMono
{
    public float burnDamage;
    public float fireElement;
    PlayerManager player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
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
        //basa = SetToSkillID(gameObject);
        basa.damage = basa.damage * player.Fire;

        //CoroutineToDestroy(gameObject, basa.lifeTime);
    }
    public void IsNeedToDestroy()
    {
        if (!basa.stats[2].isTrigger)
        {
            Destroy(gameObject);
        }
    }
    //public void CreateWave()
    //{
    //    FireWave a = Instantiate(this, transform.position, Quaternion.identity);
    //    a.basa.damage = basa.damage;
    //    a.fireElement = player.Fire;
    //    a.burnDamage = burnDamage;
    //}
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.transform.position;
        //transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * 20, transform.localScale.y + Time.deltaTime * 20, transform.localScale.z + Time.deltaTime * 20);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponentInParent<ElementActiveDebuff>() != null && !collision.GetComponentInParent<ElementActiveDebuff>().IsActive("isFire", true))
            {
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isFire", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isFire", true, false);
            }
            collision.GetComponent<HealthPoint>().TakeDamage((basa.damage * collision.GetComponent<HealthPoint>().Water) / collision.GetComponent<HealthPoint>().Fire);
            GameManager.Instance.FindStatName("fireWaveDamage", (basa.damage * collision.GetComponent<HealthPoint>().Water) / collision.GetComponent<HealthPoint>().Fire);
            if (burnDamage != 0 && collision != null)
            {
                collision.GetComponent<HealthPoint>().isBurn = true;
                collision.GetComponent<HealthPoint>().burnTime = 3;
                collision.GetComponent<HealthPoint>().burnDamage = burnDamage;
            }
        }
    }
}
