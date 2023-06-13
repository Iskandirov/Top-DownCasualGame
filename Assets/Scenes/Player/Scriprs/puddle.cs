using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puddle : MonoBehaviour
{
    public float liveTime;
    public float damageTick;
    public float damageTickMax;

    public Collider2D[] enemies;

    public float damage;

    public Sprite[] lights;
    public float radius;
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(gameObject.transform.position, radius);
    }
    // Start is called before the first frame update
    void Start()
    {
        //zzap.Play();
    }

    // Update is called once per frame
    void Update()
    {
        liveTime -= Time.deltaTime;
        damageTick -= Time.deltaTime;
        if (damageTick <= 0)
        {

            enemies = Physics2D.OverlapCircleAll(gameObject.transform.position, radius, 3);

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    if (enemies[i].GetComponent<HealthPoint>())
                    {

                        enemies[i].GetComponent<HealthPoint>().ChangeToKick();
                        enemies[i].GetComponent<HealthPoint>().healthPoint -= damage;
                    }
                    else if (enemies[i].GetComponent<HealthBossPart>())
                    {
                        enemies[i].GetComponent<HealthBossPart>().ChangeToKick();
                        enemies[i].GetComponent<HealthBossPart>().healthPoint -= damage;
                    }
                }
            }
            damageTick = damageTickMax;
        }
        if (liveTime <= 0)
        {

            Destroy(gameObject);
        }
    }

}
