using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ImpulsSpawner : MonoBehaviour
{
    public Impuls impuls;
    public float step;
    public float stepMax;
    public float powerGrow;
    public bool isFour;
    public bool isFive;
    // Start is called before the first frame update
    void Start()
    {
        step = gameObject.GetComponent<CDSkillObject>().CD;
    }

    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
        {

            Impuls a = Instantiate(impuls, transform.position, Quaternion.identity);
            a.powerGrow = powerGrow;
            a.isFour = isFour;
            a.isFive = isFive;
            step = stepMax;
        }
    }
}
