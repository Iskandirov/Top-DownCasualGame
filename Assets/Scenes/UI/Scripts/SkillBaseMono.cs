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
        GameObject foundObj = GameObject.Find(obj.name+"(Clone)");
        if (foundObj != null)
        {
            Destroy(foundObj);
        }
    }
    public void CoroutineToDestroy(GameObject obj,float lifeTime)
    {
        StartCoroutine(SimpleDestroy(obj, lifeTime));
    }
    public IEnumerator SimpleDestroy(GameObject obj, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
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
        Elements = new List<status>(stat);
        basa = setter;
        skillId = id;
    }
    public void CreateSpellByType(string type,SkillBaseMono obj, SkillBaseMono objInfo,int currentLevel, float dmgMultiplier)
    {
        switch (type)
        {
            case "base":
                CreateBaseSpell(obj, objInfo, currentLevel, dmgMultiplier);
                break;
            case "aoe":
                CreateAOESpell(obj, objInfo, currentLevel, dmgMultiplier);
                break;
            default:
                CreateBaseSpell(obj, objInfo, currentLevel, dmgMultiplier);
                break;
        }
        
    }
    public void CreateBaseSpell(SkillBaseMono mono, SkillBaseMono objInfo, int currentLevel, float dmgMultiplier)
    {
        player = PlayerManager.instance;
        SkillBaseMono a = Instantiate(mono, new Vector3(player.objTransform.position.x, player.objTransform.position.y, 0f), Quaternion.identity);
        a.basa = objInfo.basa;
        a.currentLevel = currentLevel;
        a.damageMultiplier = dmgMultiplier;
        a.Elements = objInfo.Elements;
        if (basa.isPassive)
        {
            a.transform.parent = player.transform;
        }
    }
    public void CreateAOESpell(SkillBaseMono mono, SkillBaseMono objInfo, int currentLevel, float dmgMultiplier) 
    {
        player = PlayerManager.instance;
        SkillBaseMono a = Instantiate(mono, new Vector3(player.objTransform.position.x + UnityEngine.Random.Range(-20, 20), player.objTransform.position.y + UnityEngine.Random.Range(-20, 20), 0f), Quaternion.identity);
        a.basa = objInfo.basa;
        a.currentLevel = currentLevel;
        a.damageMultiplier = dmgMultiplier;
        a.Elements = objInfo.Elements;
        if (basa.isPassive)
        {
            a.transform.parent = player.transform;
        }
    }
}