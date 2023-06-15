using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerEnemy : MonoBehaviour
{
    public Summon summon;
    public float step;
    public float stepMax;
    public float lifeTime;
    public bool isThree;
    public float attackSpeed;
    public bool isFive;

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
            Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 16f);

            foreach (Collider2D collider in colliders)
            {
                if (collider.isTrigger != true && collider.CompareTag("Enemy"))
                {
                    if (collider.GetComponent<HealthPoint>())
                    {
                        Summon a = Instantiate(summon, transform.position, Quaternion.identity);
                        a.enemy = collider.gameObject;
                        a.lifeTime = lifeTime;
                        a.isThree = isThree;
                        a.attackSpeed = attackSpeed;
                        step = stepMax;

                        //призивати об'Їкт з скр≥птом €кий буде сумонити ворога
                        break;  // «упинити пошук п≥сл€ знайденн€ першого об'Їкта Enemy
                    }
                }
            }
        }
    }
}
