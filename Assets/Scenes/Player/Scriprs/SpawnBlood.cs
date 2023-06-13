using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlood : MonoBehaviour
{
    public puddle puddle;
    public float step;
    public float stepMax;
    public float radius;
    public float damage;
    public float numOfChair;
    public float damageTickMax;
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
            for (int i = 0; i < numOfChair; i++)
            {
                puddle a = Instantiate(puddle, new Vector3(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-20, 20), 1.9f), Quaternion.identity);
                a.damage = damage;
                a.radius += radius;
                a.gameObject.transform.localScale = new Vector2(a.gameObject.transform.localScale.x + radius * 2, a.gameObject.transform.localScale.y + radius * 2);
                a.damageTickMax = damageTickMax;
            }
            step = stepMax;
        }
    }
}
