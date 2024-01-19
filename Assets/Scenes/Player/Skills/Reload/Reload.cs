using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reload : SkillBaseMono
{
    public List<CDSkills> spells;
    
    // Start is called before the first frame update
    void Start()
    {
        spells = FindObjectsOfType<CDSkills>().ToList();
        //basa = SetToSkillID(gameObject);
        for(int i = 0; i < spells.Count;i++)
        {
            if (spells[i].abilityId == 0 || spells[i].abilityId == 6)
            {
                spells.RemoveAt(i);
            }
        }
        //FindAllActiveSkill();
        int randomedSkill;
        if (spells != null)
        {
            for (int y = 0; y < basa.countObjects; y++)
            {
                randomedSkill = Random.Range(0, spells.Count);
                spells[randomedSkill].skillCD = 0;
            }
            GameManager.Instance.FindStatName("skillsReloaded", 1);
        }
    }
}
