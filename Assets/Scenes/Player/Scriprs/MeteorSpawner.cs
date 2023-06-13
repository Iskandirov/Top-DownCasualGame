using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public Meteor meteor;
    public float step;
    public float stepMax;
    public float damage;
    public bool isThree;
    public bool isFour;
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
            Meteor a = Instantiate(meteor, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
            a.damage = damage;
            a.isFour = isFour;
            if (isThree)
            {
                Meteor b = Instantiate(meteor, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
                b.damage = damage; 
                b.isFour = isFour; 
                Meteor c = Instantiate(meteor, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
                c.damage = damage;
                c.isFour = isFour;
                if (isFive)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Meteor x = Instantiate(meteor, new Vector2(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20)), Quaternion.identity);
                        x.damage = damage;
                        x.isFour = isFour;
                    }
                }
            }
            step = stepMax;
        }
    }
}
