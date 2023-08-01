using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class HealSpawner : MonoBehaviour
{
    public HealActive healObj;
    public float step;
    public float stepMax;
    public float heal;
    public bool isLevelTwo;
    ElementsCoeficients grassElement;
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        grassElement = transform.root.GetComponent<ElementsCoeficients>();
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

            HealActive a = Instantiate(healObj, transform.position, Quaternion.identity);
            a.heal = heal;
            a.isLevelTwo = isLevelTwo;
            a.Grass = grassElement.Grass;
            step = stepMax;
        }

    }
}
