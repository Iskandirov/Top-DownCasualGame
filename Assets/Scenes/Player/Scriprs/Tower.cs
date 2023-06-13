using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float lifeTime;
    public float spawnTick;
    public float spawnTickMax;
    public TowerWave damageObj;
    public GameObject bomb;
    public bool isThree;
    public bool isFive;
    Collider2D[] colliders;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isFive)
        {
            colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 16f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.isTrigger != true && collider.CompareTag("Enemy") && collider.transform.parent.gameObject != gameObject)
                {
                    if (collider.GetComponent<HealthPoint>())
                    {
                        collider.transform.root.GetComponent<Forward>().player = gameObject;
                        collider.transform.root.GetComponent<Forward>().enemyFinded = true;
                    }
                }
            }
        }
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            if (isThree)
            {
                Instantiate(bomb, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        spawnTick -= Time.deltaTime;
        if (spawnTick <= 0)
        {
            TowerWave a = Instantiate(damageObj, transform.position, Quaternion.identity);
            a.player = gameObject;
            spawnTick = spawnTickMax;
        }

    }
}
