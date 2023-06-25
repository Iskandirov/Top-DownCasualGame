using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    public Shield shield;
    public float step;
    public float stepMax;
    public float ShieldHP;
    public bool isThree;
    public bool isFour;
    public bool isFive;
    ElementsCoeficients dirtElement;
    // Start is called before the first frame update
    void Start()
    {
        dirtElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
    }

    // Update is called once per frame
    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
        {
            Shield a = Instantiate(shield, transform.position, Quaternion.identity);
            a.healthShield = ShieldHP;
            a.isThreeLevel = isThree;
            a.isFourLevel = isFour;
            a.isFiveLevel = isFive;
            a.dirtElement = dirtElement.Dirt;
            step = stepMax;
        }
    }
}
