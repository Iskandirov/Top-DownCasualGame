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
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        dirtElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
        StartCoroutine(SetBumberToSkill());
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
        buttonActivateSkill = gameObject.GetComponent<CDSkillObject>().num + 1;
        keyCode = (KeyCode)((int)KeyCode.Alpha0 + buttonActivateSkill);
    }
    // Update is called once per frame
    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0 && Input.GetKeyDown(keyCode))
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
