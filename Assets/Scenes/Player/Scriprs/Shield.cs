using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float healthShield;
    public float healthShieldMissed;
    public float lifeTime;
    public GameObject player;
    public GameObject slowObj;
    public GameObject rockObj;
    public bool isThreeLevel;
    public bool isFourLevel;
    public bool isFiveLevel;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Health>().gameObject;
        if (isFourLevel)
        {
            Instantiate(slowObj, transform.position, Quaternion.identity, transform);
        }
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
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
                List<GameObject> enemyObjects = new List<GameObject>();

                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        // Перевірка, чи зіткнення відбулось з іншим об'єктом (не самим собою)
                        if (collider.gameObject != newObject)
                        {
                            enemyObjects.Add(collider.gameObject);
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
