using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobmExplode : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f);

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    if (collider.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || collider.GetComponent<HealthBossPart>())
                    {
                        collider.GetComponent<HealthBossPart>().healthPoint -= damage;
                        collider.GetComponent<HealthBossPart>().ChangeToKick();

                    }
                    else if (!collider.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || !collider.GetComponent<HealthBossPart>())
                    {
                        collider.GetComponent<HealthPoint>().healthPoint -= damage;
                        collider.GetComponent<HealthPoint>().ChangeToKick();
                    }
                }
                else if (collider.CompareTag("Barrel"))
                {
                    collider.gameObject.GetComponent<ObjectHealth>().health -= damage;
                }
            }
            Destroy(gameObject);
        }
    }
}
