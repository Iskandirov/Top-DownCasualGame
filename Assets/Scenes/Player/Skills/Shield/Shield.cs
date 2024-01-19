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
    public bool isThreeLevel;
    public bool isFourLevel;
    public bool isFiveLevel;
    public float dirtElement;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        //basa = SetToSkillID(gameObject);
        healthShield = basa.damage;
        player.shildActive = true;
        dirtElement = player.Dirt;
        if (isFourLevel)
        {
            SlowArea a = Instantiate(slowObj, transform.position, Quaternion.identity, transform);
            a.dirtElement = dirtElement;
        }
        StartCoroutine(TimerSpell());
    }
    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        player.shildActive = false;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.transform.position;
        if (healthShieldMissed > 20f)
        {
            float i = Mathf.Floor(healthShieldMissed / 20f);
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
        if (healthShield <= 0) 
        {
            Destroy(gameObject);
        }
    }
}
