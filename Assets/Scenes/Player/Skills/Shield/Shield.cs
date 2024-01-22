using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : SkillBaseMono
{
    public float healthShield;
    public float healthShieldMissed;
    PlayerManager player;
    public SlowArea slowObj;
    public float rockDamage;
    public GameObject rockObj;
    public float dirtElement;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        transform.localScale = new Vector2(transform.localScale.x + basa.radius, transform.localScale.y + basa.radius);
        healthShield = basa.damage;
        player.shildActive = true;
        dirtElement = player.Dirt;
        if (basa.stats[3].isTrigger)
        {
            SlowArea a = Instantiate(slowObj, transform.position, Quaternion.identity, transform);
            a.dirtElement = dirtElement;
        }
        CoroutineToDestroy(gameObject, basa.lifeTime);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.transform.position;
        if (basa.stats[4].isTrigger)
        {
            if (healthShieldMissed > 10f)
            {
                float i = Mathf.Floor(healthShieldMissed / 10f);
                for (int y = 0; y < i; y++)
                {
                    GameObject newObject = Instantiate(rockObj, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);


                    // Перевірка зіткнень з навколишніми об'єктами
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(newObject.transform.position, newObject.GetComponent<Collider2D>().bounds.extents.x);

                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.CompareTag("Enemy"))
                        {
                            // Перевірка, чи зіткнення відбулось з іншим об'єктом (не самим собою)
                            if (collider.gameObject != newObject)
                            {
                                collider.GetComponent<HealthPoint>().healthPoint -= (rockDamage * dirtElement * collider.GetComponent<HealthPoint>().Dirt) / collider.GetComponent<HealthPoint>().Grass;
                                // Здійснюйте необхідні дії при зіткненні об'єкта
                                Debug.Log("Object collided with: " + collider.name);
                            }
                        }
                    }
                }
                healthShieldMissed = 0;
            }
        }
        if (healthShield <= 0) 
        {
            Destroy(gameObject);
        }
    }
}
