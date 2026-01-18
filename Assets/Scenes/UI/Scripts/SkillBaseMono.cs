using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillBase 
{
    [SerializeField]
    public List<Data> stats;
    public CDSkills skill;
    //Base
    public SkillBaseMono objToSpawn;
    public float stepMax;
    public float lifeTime;
    public float damage;
    public string spawnSFXName;
    public string despawnSFXName;
    ////AOE
    public float damageTickMax;
    public float countObjects;
    public float radius;
    ////Trigger
    public bool isPassive;
    public float spawnDelay;
    public List<Data> GetStats()
    {
        return stats;
    }
}
[Serializable]
public class SkillBaseMono : MonoBehaviour
{
    [SerializeField]
    public List<status> Elements;

    public SkillBase basa;
    public int skillId;
    public int currentLevel;
    [HideInInspector]
    public float damageMultiplier = 1f;
    public PlayerManager player;
    public void DestroyObject(GameObject obj)
    {
            Destroy(obj.gameObject);
    }
    public void CoroutineToDestroy(GameObject obj,float lifeTime)
    {
        StartCoroutine(SimpleDestroy(obj, lifeTime));
    }
    public IEnumerator SimpleDestroy(GameObject obj, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        AudioManager.instance.PlaySFX(basa.despawnSFXName);
        Destroy(obj);
    }
    public float StabilizateCurrentReload(float currentReload,float newStep)
    {
        if (currentReload > 0)
        {
            return newStep;
        }
        return 0;
    }
    public void Set(SkillBase setter,int id, List<status> stat)
    {
        player = PlayerManager.instance;
        Elements = new List<status>(stat);
        basa = setter;
        skillId = id;
    }
    public enum SpellType { Base, AOE }

    public SkillBaseMono CreateSpellByType(SpellType type, SkillBaseMono obj, SkillBaseMono objInfo, int currentLevel, float dmgMultiplier)
    {
        Vector3 spellPos = new Vector3(player.objTransform.position.x, player.objTransform.position.y, 0f);
        Vector3 aoePos = new Vector3(player.objTransform.position.x + UnityEngine.Random.Range(-20, 20), player.objTransform.position.y + UnityEngine.Random.Range(-20, 20), 0f);
        SkillBaseMono skillBaseMono = null;
        switch (type)
        {
            case SpellType.Base: skillBaseMono = CreateSpell(obj, objInfo, currentLevel, dmgMultiplier, spellPos); break;
            case SpellType.AOE: skillBaseMono = CreateSpell(obj, objInfo, currentLevel, dmgMultiplier, aoePos); break;
        }
        return skillBaseMono;
    }
    private SkillBaseMono CreateSpell(SkillBaseMono prefab, SkillBaseMono objInfo, int currentLevel, float dmgMultiplier, Vector3 position)
    {
        var spell = Instantiate(prefab, position, Quaternion.identity);
        spell.basa = objInfo.basa;
        spell.currentLevel = currentLevel;
        spell.damageMultiplier = dmgMultiplier;
        spell.Elements = objInfo.Elements;

        if (basa.isPassive)
            spell.transform.parent = player.transform;

        return spell;
    }
}