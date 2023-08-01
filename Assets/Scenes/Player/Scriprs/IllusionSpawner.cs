using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class IllusionSpawner : MonoBehaviour
{
    public Illusion illusion;
    public float step;
    public float stepMax;
    public float lifeTime;
    public bool isTwo;
    public bool isFour;
    public bool isFive;
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        step = gameObject.GetComponent<CDSkillObject>().CD;
        StartCoroutine(SetBumberToSkill());
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
        buttonActivateSkill = gameObject.GetComponent<CDSkillObject>().num + 1;
        keyCode = (KeyCode)((int)KeyCode.Alpha0 + buttonActivateSkill);
    }

    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {

            Illusion a = Instantiate(illusion, transform.position, Quaternion.identity);
            a.x = -10;
            a.y = 10;
            a.xZzap = 2;
            a.yZzap = -10;
            a.angle = 0;
            a.lifeTime = lifeTime;
            a.isFive = isFive;
            if (isTwo)
            {
                Illusion b = Instantiate(illusion, transform.position, Quaternion.identity);
                b.x = -5;
                b.y = -5;
                b.xZzap = 10;
                b.yZzap = 0;
                b.angle = 112;
                b.lifeTime = lifeTime;
                b.isFive = isFive;

                if (isFour)
                {
                    Illusion c = Instantiate(illusion, transform.position, Quaternion.identity);
                    c.x = 10;
                    c.y = 0;
                    c.xZzap = -10;
                    c.yZzap = 5;
                    c.angle = 240;
                    c.lifeTime = lifeTime;
                    c.isFive = isFive;
                }
            }
            step = stepMax;
        }
    }
}
