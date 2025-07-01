using FSMC.Runtime;
using System.Collections;
using UnityEngine;

public class puddle : SkillBaseMono
{
    public Collider2D[] enemies;
  
    Vector3 halfExtents;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

    }
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        if (basa.stats[1].isTrigger)
        {
            basa.radius += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.damage *= basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
       
        if (basa.stats[4].isTrigger)
        {
            basa.damageTickMax -= basa.stats[4].value;
            basa.stats[4].isTrigger = false;
        }

        transform.localScale = new Vector2((transform.localScale.x + basa.radius) * player.Dirt, (transform.localScale.y + basa.radius) * player.Dirt);
        halfExtents = new Vector3(transform.localScale.x * 4f, transform.localScale.y, transform.localScale.z);


        StartCoroutine(CastSpell());
        CoroutineToDestroy(gameObject, basa.lifeTime);
    }
   
    private IEnumerator CastSpell()
    {
        while (true)
        {
            enemies = Physics2D.OverlapBoxAll(transform.position, halfExtents, 0f);

            foreach (var enemy in enemies)
            {
                ElementActiveDebuff debuff = enemy.GetComponentInParent<ElementActiveDebuff>();


                if (enemy != null && enemy.CompareTag("Enemy") && debuff != null)
                {

                    FSMC_Executer objHealth = enemy.GetComponent<FSMC_Executer>();

                    if (debuff != null)
                    {
                        debuff.ApplyEffect(Elements.status.Water, 5);
                        debuff.ApplyEffect(Elements.status.Dirt, 5);
                    }
                    objHealth.TakeDamage(basa.damage / debuff.elements.CurrentStatusValue(Elements.status.Electricity) * player.Water / debuff.elements.CurrentStatusValue(Elements.status.Water) * debuff.elements.CurrentStatusValue(Elements.status.Dirt), damageMultiplier);
                    GameManager.Instance.FindStatName("puddleDamage", (basa.damage / debuff.elements.CurrentStatusValue(Elements.status.Electricity)) * player.Water 
                        / debuff.elements.CurrentStatusValue(Elements.status.Water) * debuff.elements.CurrentStatusValue(Elements.status.Dirt));
                }
            }

            yield return new WaitForSeconds(basa.damageTickMax);
        }
    }
}
