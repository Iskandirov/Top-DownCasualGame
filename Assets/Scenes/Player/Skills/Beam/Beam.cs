using FSMC.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Beam : SkillBaseMono
{
    public float tick;
    public float addToAndle;
    public bool isTwo;
    public SpriteRenderer img;
    Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objTransform = transform;
        if (basa.stats[1].isTrigger)
        {
            basa.countObjects += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
            CreateBeam(-90, 0, -5);
            CreateBeam(90, 0, 5);
        }
        if (basa.stats[2].isTrigger)
        {
            basa.damage += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.lifeTime += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            basa.skill.skill.stepMax -= basa.stats[4].value;
            basa.stats[4].isTrigger = false;
        }
        tick = basa.damageTickMax;
        basa.damage = basa.damage * player.Steam;
        objTransform.localScale = new Vector3(objTransform.localScale.x + basa.radius, objTransform.localScale.y + basa.radius);

        CoroutineToDestroy(gameObject, basa.lifeTime);
        IsThereAnotherBeam();
    }
    public void IsThereAnotherBeam()
    {
        int angle = -90;
        List<Beam> beams = FindObjectsOfType<Beam>().ToList();
        if (beams.Count > 1)
        {
            foreach (Beam beam in beams)
            {
                beam.addToAndle = angle;
                angle += 90;
            }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        // Визначаємо відстань від гравця до позиції курсора
        Vector3 direction = player.GetMousDirection(player.objTransform.position);
        direction = direction * basa.radius;

        // Рухаємо об'єкт до кінцевої позиції
        objTransform.position = new Vector3(player.ShootPoint.transform.position.x, player.ShootPoint.transform.position.y, 5f);

        // Повертаємо коло в напрямку курсора
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        objTransform.rotation = Quaternion.AngleAxis(angle + addToAndle, Vector3.forward);

        tick -= Time.deltaTime;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Damage(collision);
        }
        else if (collision.CompareTag("Barrel") && collision != null)
        {
            collision.GetComponent<ObjectHealth>().TakeDamage();
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (tick <= 0)
            {
                tick = basa.damageTickMax;
                Damage(collision);
            }
        }
        else if (collision.CompareTag("Barrel") && collision != null)
        {
            collision.GetComponent<ObjectHealth>().TakeDamage();
        }
    }
    void Damage(Collider2D collision)
    {
        FSMC_Executer objHealt = collision.GetComponent<FSMC_Executer>();
        ElementActiveDebuff debuff = collision.GetComponent<ElementActiveDebuff>();
        debuff.StartCoroutine(debuff.EffectTime(Elements.status.Steam, 5));
        
        objHealt.TakeDamage((basa.damage * debuff.elements.CurrentStatusValue(Elements.status.Steam)) / debuff.elements.CurrentStatusValue(Elements.status.Cold));

        GameManager.Instance.FindStatName("beamDamage", (basa.damage * debuff.elements.CurrentStatusValue(Elements.status.Steam))
        / debuff.elements.CurrentStatusValue(Elements.status.Cold));
    }
    private void CreateBeam(float angle, float x, float y)
    {
        Beam a = Instantiate(this, new Vector2(objTransform.position.x + x, objTransform.position.y + y), Quaternion.identity);
        a.basa.damage = basa.damage * player.Steam;
        a.addToAndle = angle;
        a.transform.localScale = new Vector3(objTransform.localScale.x, objTransform.localScale.y);
    }
}
