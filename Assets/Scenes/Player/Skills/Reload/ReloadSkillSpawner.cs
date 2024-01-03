using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ReloadSkillSpawner : MonoBehaviour
{
    public Reload reload;
    public float step;
    public float stepMax;
    public int count;
    public bool isLevelFive;

    public SkillCDLink spellObj;
    public List<CDSkillObject> spells;
    public List<CDSkillObject> spellsToRemove;
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
    void FindAllActiveSkill()
    {
        spells = spellObj.GetComponentsInChildren<CDSkillObject>().ToList();
        foreach (var spell in spells)
        {
            if (spell.isPassive || spell == gameObject.GetComponent<CDSkillObject>())
            {
                spellsToRemove.Add(spell);
                //spells.RemoveAt(spells.IndexOf(spell));
            }
        }
        foreach (var remove in spellsToRemove)
        {
            if (spells.Contains(remove))
            {
                spells.RemoveAt(spells.IndexOf(remove));
            }
        }
        spellsToRemove.Clear();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        step -= Time.fixedDeltaTime;
       
        if (isLevelFive)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                var spell = spells[i];

                if (spell != null && spell.valueFieldStep != null)
                {
                    spell.CD *= 0.5f;
                    spells[i] = spell; // Оновлення оригінального об'єкта в колекції
                    spells[i].SetCD();
                }
            }
            isLevelFive = false;
        }
    }
    private void Update()
    {
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {
            FindAllActiveSkill();
            Reload a = Instantiate(reload, transform.position, Quaternion.identity);
            a.count = count;
            a.spells = spells;
            step = stepMax;
        }
    }
}
