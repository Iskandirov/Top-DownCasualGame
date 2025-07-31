using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zzap : MonoBehaviour
{
    public float lifeTime;
    public float damageTick;
    public float damageTickMax;
    public float electicElement;
    public EnemySpawner enemies;
    public SkillBase basa;
    public List<Collider2D> enemiesColliders = new List<Collider2D>();
    // Start is called before the first frame update
    void Start()
    {
        electicElement = PlayerManager.instance.Electricity;
        enemies = FindAnyObjectByType<EnemySpawner>();
        StartCoroutine(DealDamage());
        StartCoroutine(TimerSpell());
        //transform.position = new Vector2(transform.position.x + x, transform.position.y + y);// +45
    }
    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
   
    IEnumerator DealDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageTick);
            if (enemiesColliders.Count > 0)
            {
                for (int i = 0; i < enemiesColliders.Count; i++)
                {
                    ElementActiveDebuff debuff = enemiesColliders[i].GetComponentInParent<ElementActiveDebuff>();
                    debuff.ApplyEffect(status.Electricity, 5);
                    enemiesColliders[i].GetComponent<FSMC_Executer>().TakeDamage(basa.damage * electicElement, 1);
                    GameManager.Instance.FindStatName("zzapDamage", basa.damage * electicElement);
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (!enemiesColliders.Contains(collision))
            {
                enemiesColliders.Add(collision);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            enemiesColliders.Remove(collision);
        }
    }
}
