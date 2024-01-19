using System.Collections;
using UnityEngine;

public class IceWall : SkillBaseMono
{
    public float cold;
    public float damageTick;
    

    void Start()
    {
        //basa = SetToSkillID(gameObject);
        damageTick = basa.damageTickMax;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1.9f; // Задаємо Z-координату для об'єкта
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = worldPosition;
        transform.localScale = new Vector2(basa.radius * PlayerManager.instance.Steam, basa.radius * PlayerManager.instance.Steam);
        cold = PlayerManager.instance.Cold;
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        Destroy(gameObject);
    }
    private void Update()
    {
        damageTick -= Time.deltaTime;
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
                StartCoroutine(collision.GetComponentInParent<Forward>().SlowEnemy(1f, 0.1f));
                if (damageTick <= 0)
                {
                    collision.GetComponent<HealthPoint>().TakeDamage((basa.damage * cold) / collision.GetComponent<HealthPoint>().Fire);
                    damageTick = basa.damageTickMax;
                }
            }
        }
    }
}
