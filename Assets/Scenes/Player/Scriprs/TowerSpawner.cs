using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{

    public Tower tower;
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
            Tower a = Instantiate(tower, new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10)), Quaternion.identity);
            a.lifeTime = lifeTime;
            a.isThree = isThree;
            a.spawnTickMax = attackSpeed;
            a.isFive = isFive;
            step = stepMax;
        }
    }
}
