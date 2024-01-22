using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Beam : SkillBaseMono
{
    public float tick;
    PlayerManager player;
    public float addToAndle;
    public bool isTwo;
    public SpriteRenderer img;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
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
        transform.localScale = new Vector3(transform.localScale.x + basa.radius, transform.localScale.y + basa.radius);

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
        Vector3 direction = player.GetMousDirection(player.transform.position);
        direction = direction * basa.radius;

        // Рухаємо об'єкт до кінцевої позиції
        transform.position = new Vector2(player.transform.position.x, player.transform.position.y);

        // Повертаємо коло в напрямку курсора
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + addToAndle, Vector3.forward);

        tick -= Time.deltaTime;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HealthPoint objHealt = collision.GetComponent<HealthPoint>();
            
            if (collision.GetComponentInParent<ElementActiveDebuff>() != null && !collision.GetComponentInParent<ElementActiveDebuff>().IsActive("isSteam", true))
            {
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, false);
            }
            objHealt.healthPoint -= (basa.damage * objHealt.Steam) / objHealt.Cold;
            GameManager.Instance.FindStatName("beamDamage", (basa.damage * objHealt.Steam) / objHealt.Cold);
        }
        else if (collision.CompareTag("Barrel") && collision != null)
        {
            collision.GetComponent<ObjectHealth>().health -= 1;
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (tick <= 0)
            {
                tick = basa.damageTickMax;
                HealthPoint objHealt = collision.GetComponent<HealthPoint>();
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isSteam", true, false);
                objHealt.healthPoint -= (basa.damage * objHealt.Steam) / objHealt.Cold;
            }
        }
        else if (collision.CompareTag("Barrel") && collision != null)
        {
            collision.GetComponent<ObjectHealth>().health -= 1;
        }
    }
    private void CreateBeam(float angle, float x, float y)
    {
        Beam a = Instantiate(this, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity);
        a.basa.damage = basa.damage * player.Steam;
        a.addToAndle = angle;
        a.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y);
    }
}
