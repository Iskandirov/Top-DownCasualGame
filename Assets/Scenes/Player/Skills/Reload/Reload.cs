using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reload : SkillBaseMono
{
    public List<CDSkills> spells;
    
    // Start is called before the first frame update
    void Start()
    {
        if (basa.stats[1].isTrigger)
        {
            basa.countObjects += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.stepMax -= basa.stats[2].value;
            basa.stats[2].isTrigger = false;
            basa.skill.skillCD -= StabilizateCurrentReload(basa.skill.skillCD, basa.stats[2].value);
        }
        if (basa.stats[3].isTrigger)
        {
            basa.countObjects += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            AllSkillRechargeLower();
            basa.stats[4].isTrigger = false;
        }

        spells = FindObjectsOfType<CDSkills>().ToList();
        for(int i = 0; i < spells.Count;i++)
        {
            if (spells[i].abilityId == 0 || spells[i].abilityId == basa.skill.number || !spells[i].isPassive)
            {
                spells.RemoveAt(i);
            }
        }
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
    void AllSkillRechargeLower()
    {
        spells = FindObjectsOfType<CDSkills>().ToList();

        //spells.Where(spell => spell.abilityId != 12);
        foreach (var skill in spells)
        {
            if (skill.abilityId != 12)
            {
                skill.skill.stepMax *= basa.stats[4].value;
            }
        }
    }
}
