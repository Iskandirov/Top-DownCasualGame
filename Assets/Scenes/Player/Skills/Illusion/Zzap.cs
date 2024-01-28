using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zzap : MonoBehaviour
{
    public float damage;
    public float x;
    public float y;
    public float lifeTime;
    public float damageTick;
    public float damageTickMax;
    public float electicElement;
    EnemyController enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = EnemyController.instance;
        StartCoroutine(TimerSpell());
        transform.position = new Vector2(transform.position.x + x, transform.position.y + y);// +45
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
                enemy.TakeDamage(collision.GetComponent<EnemyState>(), damage * electicElement);
                GameManager.Instance.FindStatName("zzapDamage", damage * electicElement);
            }
            damageTick = damageTickMax;
        }
       
    }
}
