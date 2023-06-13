using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{

    public Collider2D[] enemies;
    public Collider2D[] enemiesToShoot;
    public GameObject lightObj;
    //public GameObject vFXLight;

    public float step;
    public float stepMax;
    public int maxEnemiesToShoot;
    public float stunTime;
    public float damage;
    // Start is called before the first frame update
    void Start()
    {
        step = gameObject.GetComponent<CDSkillObject>().CD;
    }

    // Update is called once per frame
    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
        {
            enemiesToShoot = new Collider2D[maxEnemiesToShoot];
            enemies = Physics2D.OverlapCircleAll(gameObject.transform.position, 16f, 3);

            if (enemies != null && enemies.Length > 0)
            {
                int enemiesCount = 0;
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].isTrigger != true && enemies[i].CompareTag("Enemy"))
                    {
                        enemiesToShoot[enemiesCount] = enemies[i];
                        enemiesCount++;

                        if (enemiesCount >= maxEnemiesToShoot)
                        {
                            break;
                        }
                    }
                }

                for (int i = 0; i < enemiesCount; i++)
                {
                    if (enemiesToShoot[i] != null)
                    {
                        if (enemiesToShoot[i].GetComponent<HealthPoint>())
                        {
                            Instantiate(lightObj, new Vector3(enemiesToShoot[i].transform.position.x,
                            enemiesToShoot[i].transform.position.y + 3), Quaternion.identity);

                            enemiesToShoot[i].GetComponent<HealthPoint>().healthPoint -= damage;
                            enemiesToShoot[i].GetComponent<HealthPoint>();
                            enemiesToShoot[i].GetComponentInParent<Forward>().isStunned = true;
                            enemiesToShoot[i].GetComponentInParent<Forward>().stunnTime = stunTime;

                            //Instantiate(vFXLight, new Vector3(enemiesToShoot[i].transform.position.x,
                            //    enemiesToShoot[i].transform.position.y), Quaternion.identity);

                            if (enemiesToShoot[i].GetComponent<HealthPoint>().IsBobs == true)
                            {
                                enemiesToShoot[i].GetComponentInChildren<Animator>().SetBool("IsHit", true);
                            }
                            else
                            {
                                enemiesToShoot[i].GetComponent<HealthPoint>().ChangeToKick();
                            }
                        }
                        else if (enemiesToShoot[i].GetComponent<HealthBossPart>())
                        {
                            Instantiate(lightObj, new Vector3(enemiesToShoot[i].transform.position.x,
                            enemiesToShoot[i].transform.position.y + 3), Quaternion.identity);

                            enemiesToShoot[i].GetComponent<HealthBossPart>().healthPoint -= damage;
                            enemiesToShoot[i].GetComponent<HealthBossPart>();
                            enemiesToShoot[i].transform.root.GetComponent<Forward>().isStunned = true;
                            enemiesToShoot[i].transform.root.GetComponent<Forward>().stunnTime = stunTime / 2;

                            //Instantiate(vFXLight, new Vector3(enemiesToShoot[i].transform.position.x,
                            //    enemiesToShoot[i].transform.position.y), Quaternion.identity);

                            enemiesToShoot[i].GetComponent<HealthBossPart>().ChangeToKick();
                        }
                        step = stepMax;
                    }
                }

            }
        }
    }
}
