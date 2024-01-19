using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ќбираЇтьс€ спелл при вибор≥ його в Levelupgrade ≥ дан≥ передаютьс€ в CDSkill при використанн≥ ск≥лла пров≥рка йде з≥ сторони CDSkill
[Serializable]
public class SkillBase 
{
    //Ѕаза дл€ кожного: ќб'Їкт, кд, шкода, чи пассивний
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
    public bool isReadyToDo;
    
    //public List<Data> GetStats()
    //{
    //    return stats;
    //}
    //public SkillBase(CDSkills skill, List<Data> stats, SkillBaseMono objToSpawn, float stepMax, float damage, float lifeTime)
    //{
    //    this.skill = skill;
    //    this.stats = stats;
    //    this.objToSpawn = objToSpawn;
    //    this.stepMax = stepMax;
    //    this.damage = damage;
    //    this.lifeTime = lifeTime;
    //}
    //public SkillBase(CDSkills skill, List<Data> stats, SkillBaseMono objToSpawn, float stepMax, float damage, float lifeTime, float radius, float countObjects, float damageTickMax)
    //{
    //    this.skill = skill;
    //    this.stats = stats;
    //    this.objToSpawn = objToSpawn;
    //    this.stepMax = stepMax;
    //    this.damage = damage;
    //    this.damageTickMax = damageTickMax;
    //    this.countObjects = countObjects;
    //    this.radius = radius;
    //    this.lifeTime = lifeTime;
    //}
}

public class SkillBaseMono : MonoBehaviour
{
    public SkillBase basa;
    public int skillId;
    public int currentLevel;
    PlayerManager player;
    //public SkillBase SetToSkillID(GameObject obj)
    //{
    //    foreach (var bas in basa)
    //    {
    //        if (bas.objToSpawn.gameObject.name + "(Clone)" == obj.name)
    //        {
    //            return bas;
    //        }
    //    }
    //    return null;
    //}
    public void DestroyObject(GameObject obj)
    {
        // «найд≥ть об'Їкт на сцен≥, €кий в≥дпов≥даЇ префабу
        GameObject foundObj = GameObject.Find(obj.name+"(Clone)");
        // якщо об'Їкт знайдено, видал≥ть його
        if (foundObj != null)
        {
            Debug.Log(1);

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
    public void Set(SkillBase setter,int id)
    {
        //Debug.Log(basa.Count);
        //Debug.Log(skillId.Count);
        basa = setter;
        skillId = id;
    }
    public void CreateSpellByType(string type,SkillBaseMono obj, SkillBaseMono objInfo,int currentLevel)
    {
        switch (type)
        {
            case "base":
                CreateBaseSpell(obj, objInfo, currentLevel);
                break;
            case "aoe":
                CreateAOESpell(obj, objInfo, currentLevel);
                break;
            default:
                CreateBaseSpell(obj, objInfo, currentLevel);
                break;
        }
        
    }
    public void CreateBaseSpell(SkillBaseMono mono, SkillBaseMono objInfo, int currentLevel)
    {
        player = PlayerManager.instance;
        SkillBaseMono a = Instantiate(mono, new Vector3(player.transform.position.x, player.transform.position.y, 1.9f), Quaternion.identity);
        //a.basa.Clear();
        // a.basa.Add(objInfo.basa[0]);
        a.basa = objInfo.basa;
        a.currentLevel = currentLevel;
        a.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    public void CreateAOESpell(SkillBaseMono mono, SkillBaseMono objInfo, int currentLevel) 
    {
        player = PlayerManager.instance;
        SkillBaseMono a = Instantiate(mono, new Vector3(player.transform.position.x + UnityEngine.Random.Range(-20, 20), player.transform.position.y + UnityEngine.Random.Range(-20, 20), 1.9f), Quaternion.identity);
        //a.basa.Clear();
        a.basa = objInfo.basa;
        a.currentLevel = currentLevel;
        a.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}