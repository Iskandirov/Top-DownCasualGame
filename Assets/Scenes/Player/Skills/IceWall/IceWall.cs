using System.Collections;
using UnityEngine;

public class IceWall : SkillBaseMono
{
    public float cold;
    public float damageTick;
    Transform objTransform;

    void Start()
    {
        objTransform = transform;
        if (basa.stats[1].isTrigger)
        {
            basa.lifeTime += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.radius += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.stepMax -= basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            basa.damage += basa.stats[4].value;
            basa.stats[4].isTrigger = false;
        }
        //basa = SetToSkillID(gameObject);
        damageTick = basa.damageTickMax;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1.9f; // Задаємо Z-координату для об'єкта
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        objTransform.position = worldPosition;
        objTransform.localScale = new Vector2(basa.radius * PlayerManager.instance.Steam, basa.radius * PlayerManager.instance.Steam);
        cold = PlayerManager.instance.Cold;
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        damageTick -= Time.fixedDeltaTime;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        IceWallDeal(collision);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        IceWallDeal(collision);
    }
    void IceWallDeal(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (collision.GetComponentInParent<Forward>())
            {
                StartCoroutine(collision.GetComponentInParent<Forward>().SlowEnemy(1f, 0.5f));
                if (damageTick <= 0)
                {
                    collision.GetComponent<HealthPoint>().TakeDamage((basa.damage * cold) / collision.GetComponent<HealthPoint>().Fire);
                    damageTick = basa.damageTickMax;
                }
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (collision.GetComponentInParent<Forward>())
            {
                StartCoroutine(collision.GetComponentInParent<Forward>().SlowEnemy(1f, 1f));
            }
        }
    }
}
