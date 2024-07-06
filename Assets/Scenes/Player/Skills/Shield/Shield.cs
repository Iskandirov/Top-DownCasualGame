using System.Collections;
using UnityEngine;


public class Shield : SkillBaseMono
{
    public float healthShield;
    public float healthShieldMissed;
    public SlowArea slowObj;
    public float rockDamage;
    public GameObject rockObj;
    public float dirtElement;
    Transform objTransform;
    public bool isPotions;
    // Start is called before the first frame update
    void Start()
    {
        if (!isPotions)
        {
            player = PlayerManager.instance;
            objTransform = transform;
            if (basa.stats[1].isTrigger)
            {
                basa.damage += basa.stats[1].value;
                basa.stats[1].isTrigger = false;
            }
            objTransform.localScale = new Vector2(objTransform.localScale.x + basa.radius, objTransform.localScale.y + basa.radius);
            healthShield = basa.damage;
            dirtElement = player.Dirt;
            if (basa.stats[3].isTrigger)
            {
                SlowArea a = Instantiate(slowObj, objTransform.position, Quaternion.identity, objTransform);
                a.dirtElement = dirtElement;
            }
            player.isInvincible = true;
            StartCoroutine(DeactivateInvincible());
            CoroutineToDestroy(gameObject, basa.lifeTime);
        }
    }
    public IEnumerator DeactivateInvincible()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        player.isInvincible = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        objTransform.position = player.objTransform.position;

        if (basa.stats[4].isTrigger && !isPotions)
        {
            if (healthShieldMissed > 10f)
            {
                float i = Mathf.Floor(healthShieldMissed / 10f);
                for (int y = 0; y < i; y++)
                {
                    GameObject newObject = Instantiate(rockObj, new Vector2(objTransform.position.x + Random.Range(-20, 20), objTransform.position.y + Random.Range(-20, 20)), Quaternion.identity);


                    // Перевірка зіткнень з навколишніми об'єктами
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(newObject.transform.position, newObject.GetComponent<Collider2D>().bounds.extents.x);

                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.CompareTag("Enemy"))
                        {
                            // Перевірка, чи зіткнення відбулось з іншим об'єктом (не самим собою)
                            if (collider.gameObject != newObject)
                            {
                                EnemyState health = collider.GetComponent<EnemyState>();
                                EnemyController.instance.TakeDamage(health, (rockDamage * dirtElement * health.GetComponent<ElementActiveDebuff>().elements.CurrentStatusValue(Elements.status.Dirt)) 
                                    / health.GetComponent<ElementActiveDebuff>().elements.CurrentStatusValue(Elements.status.Grass));
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
            player.isInvincible = false;
            Destroy(gameObject);
        }
    }
}
