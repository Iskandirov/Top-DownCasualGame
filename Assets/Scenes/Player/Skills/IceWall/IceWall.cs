using FSMC.Runtime;
using System.Collections;
using UnityEngine;

public class IceWall : SkillBaseMono
{
    public float cold;
    public float damageTick;
    Transform objTransform;
    private void Start()
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
            FSMC_Executer enemy = collision.GetComponent<FSMC_Executer>();
            ElementActiveDebuff debuff = collision.GetComponent<ElementActiveDebuff>();
            debuff.StartCoroutine(debuff.EffectTime(Elements.status.Cold, 5));

            enemy.SetFloat("SlowTime",1f);
            enemy.SetFloat("SlowPercent",.3f);
            enemy.SetCurrentState("Slow");
            if (damageTick <= 0)
            {
                enemy.TakeDamage(basa.damage * cold  / collision.GetComponent<ElementActiveDebuff>().elements.CurrentStatusValue(Elements.status.Fire));
                damageTick = basa.damageTickMax;
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            FSMC_Executer enemy = collision.GetComponent<FSMC_Executer>();
            enemy.SetFloat("SlowTime", 0.1f);
            enemy.SetFloat("SlowPercent", .9f);
            enemy.SetCurrentState("Slow");
        }
    }
}
