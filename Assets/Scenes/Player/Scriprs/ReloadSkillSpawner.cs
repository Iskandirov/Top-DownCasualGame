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
    // Start is called before the first frame update
    void Start()
    {
        step = gameObject.GetComponent<CDSkillObject>().CD;
        spells = FindObjectOfType<SkillCDLink>().gameObject;
        spellsObj = spells.GetComponentsInChildren<CDSkillObject>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
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
