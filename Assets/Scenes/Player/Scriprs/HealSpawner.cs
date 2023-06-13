using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSpawner : MonoBehaviour
{
    public HealActive healObj;
    public float step;
    public float stepMax;
    public float heal;
    public bool isLevelTwo;
    // Start is called before the first frame update
    void Start()
    {
        step = gameObject.GetComponent<CDSkillObject>().CD;
    }

    // Update is called once per frame
    void Update()
    {
        step-= Time.deltaTime;
        if (step <= 0)
        {
            HealActive a = Instantiate(healObj, transform.position, Quaternion.identity);
            a.heal = heal;
            a.isLevelTwo = isLevelTwo;
            step = stepMax;
        }
    }
}
