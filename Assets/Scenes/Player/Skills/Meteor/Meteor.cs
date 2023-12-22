using System.Collections;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float lifeTime;
    public float damageTick;
    public float damageTickMax;
    public float damage;
    public bool isFour;
    public float fireDirt;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerSpell());
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
                collision.GetComponent<HealthPoint>().TakeDamage((damage * fireDirt * collision.GetComponent<HealthPoint>().Water) / collision.GetComponent<HealthPoint>().Fire);
                GameManager.Instance.FindStatName("meteorDamage", (damage * fireDirt * collision.GetComponent<HealthPoint>().Water) / collision.GetComponent<HealthPoint>().Fire);
                damageTick = damageTickMax;
            }
            else if (collision.CompareTag("Barrel") && collision != null)
            {
                collision.GetComponent<ObjectHealth>().health -= 1;
                damageTick = damageTickMax;

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
