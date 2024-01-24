using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWave : SkillBaseMono
{
    public float burnDamage;
    public float fireElement;
    PlayerManager player;
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
            ElementActiveDebuff element = collision.GetComponentInParent<ElementActiveDebuff>();
            HealthPoint health = collision.GetComponent<HealthPoint>();

            if (element != null && !element.IsActive("isFire", true))
            {
                element.SetBool("isFire", true, true);
                element.SetBool("isFire", true, false);
            }

            health.TakeDamage((basa.damage * health.Water) / health.Fire);
            GameManager.Instance.FindStatName("fireWaveDamage", (basa.damage * health.Water) / health.Fire);

            if (burnDamage != 0 && collision != null)
            {
                health.isBurn = true;
                health.burnTime = 3;
                health.burnDamage = burnDamage;
            }
        }
    }
}
