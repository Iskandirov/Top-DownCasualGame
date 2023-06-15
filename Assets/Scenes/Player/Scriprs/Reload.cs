using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reload : MonoBehaviour
{
    public GameObject spells;
    public List<CDSkillObject> spellsObj;
    public float lifeTime;
    public int count;
    // Start is called before the first frame update
    void Start()
    {
        spells = FindObjectOfType<SkillCDLink>().gameObject;
        spellsObj = spells.GetComponentsInChildren<CDSkillObject>().ToList();
        int i = Random.Range(0, spellsObj.Count);
        if (spellsObj != null && spellsObj.Count != 1 && spellsObj[i].valueFieldStep != null && spellsObj[i].transform.root.name != "SkillCDSpawner")
        {
            for (int y = 0; y < count; y++)
            {
                spellsObj[i].valueFieldStep.SetValue(spellsObj[i].monoStep, 0.01f + y / 100);
            }
        }
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
