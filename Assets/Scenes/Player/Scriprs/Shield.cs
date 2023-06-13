using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float healthShield;
    public float healthShieldЬMissed;
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
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        if (healthShieldЬMissed > 20f)
        {
            float i = Mathf.Floor(healthShieldЬMissed / 20f);
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
            healthShieldЬMissed = 0;
        }
        lifeTime -= Time.deltaTime;
        if (healthShield <= 0 || lifeTime <= 0) 
        {
            Destroy(gameObject);
        }
    }
}
