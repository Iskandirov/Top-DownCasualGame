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
    ElementsCoeficients grassElement;
    // Start is called before the first frame update
    void Start()
    {
        grassElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;

    }

    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
        {

            HealActive a = Instantiate(healObj, transform.position, Quaternion.identity);
            a.heal = heal;
            a.isLevelTwo = isLevelTwo;
            a.Grass = grassElement.Grass;
            step = stepMax;
        }

    }
}
