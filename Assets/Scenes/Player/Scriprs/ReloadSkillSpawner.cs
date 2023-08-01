using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReloadSkillSpawner : MonoBehaviour
{
    public Reload reload;
    public float step;
    public float stepMax;
    public int count;
    public bool isLevelFive;

    public GameObject spells;
    public List<CDSkillObject> spellsObj;
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        step = gameObject.GetComponent<CDSkillObject>().CD;
        spells = FindObjectOfType<SkillCDLink>().gameObject;
        spellsObj = spells.GetComponentsInChildren<CDSkillObject>().ToList();
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
            Reload a = Instantiate(reload, transform.position, Quaternion.identity);
            a.count = count;
            step = stepMax;
        }
        if (isLevelFive)
        {
            for (int i = 0; i < spellsObj.Count; i++)
            {
                var spell = spellsObj[i];

                if (spell != null && spell.valueFieldStep != null)
                {
                    spell.CD *= 0.5f;
                    spellsObj[i] = spell; // Оновлення оригінального об'єкта в колекції
                    spellsObj[i].SetCD();
                }
            }
            isLevelFive = false;
        }
    }
}
