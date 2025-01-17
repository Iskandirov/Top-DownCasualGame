using FSMC.Runtime;
using System.Collections;
using UnityEngine;

public class Zzap : MonoBehaviour
{
    public float damage;
    public float lifeTime;
    public float damageTick;
    public float damageTickMax;
    public float electicElement;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerSpell());
        //transform.position = new Vector2(transform.position.x + x, transform.position.y + y);// +45
    }
    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        damageTick -= Time.fixedDeltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageTick <= 0)
        {
            if (collision.CompareTag("Enemy"))
            {
                ElementActiveDebuff debuff = collision.GetComponentInParent<ElementActiveDebuff>();
                if (debuff != null)
                {
                    debuff.StartCoroutine(debuff.EffectTime(Elements.status.Electricity, 5));
                }
                collision.GetComponent<FSMC_Executer>().TakeDamage(damage * electicElement);
                GameManager.Instance.FindStatName("zzapDamage", damage * electicElement);
            }
            damageTick = damageTickMax;
        }
       
    }
}
