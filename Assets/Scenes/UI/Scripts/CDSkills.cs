using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Data 
{
    public bool isTrigger;
    public float value;
}
[DefaultExecutionOrder(12)]
public class CDSkills : MonoBehaviour
{
    [HideInInspector]
    public List<Data> stats;

    public int currentStatLevel;


    public string type;
    public SkillBase skill;
    public SkillBaseMono skillMono;
    public float skillCD;
    public Image spriteCD;
    public int number;
    public GameObject text;
    public int abilityId;
    public SkillBaseMono objToSpawn;

    public float step;
    public bool isPassive;

    public KeyCode keyCode;
    TextMeshProUGUI cDText;
    // Update is called once per frame
    void Start()
    {
        skill.skill = this;
        skillMono.Set(skill,number);
        cDText = text.GetComponent<TextMeshProUGUI>();
        StartCoroutine(SetBumberToSkill());
        if (abilityId == 0)
        {
            skill.stepMax = FindObjectOfType<PlayerManager>().attackSpeed;
            skillCD = skill.stepMax;
            skill.damage = FindObjectOfType<PlayerManager>().damageToGive;
        }
    }
    void FixedUpdate()
    {
        spriteCD.fillAmount = skillCD / skill.stepMax;
        skillCD -= Time.fixedDeltaTime;
        if (skillCD <= 0)
        {
            text.SetActive(false);
        }
        else
        {
            text.SetActive(true);
        }
        cDText.text = skillCD.ToString("0.0");
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
    }
    private IEnumerator WaitToAnotherObject(int count,float delay)
    {
        float followUpMultiplier = 1f;

        for (int i = 0; i < count - 1; i++)
        {
            yield return new WaitForSeconds(delay);
            followUpMultiplier = 1f;
            followUpMultiplier = followUpMultiplier / (i + 1.3f);
            skillMono.CreateSpellByType(type, skill.objToSpawn, skillMono, currentStatLevel, followUpMultiplier);
        }
    }
    void Spawn(int counter)
    {
        skillMono.CreateSpellByType(type, skill.objToSpawn, skillMono, currentStatLevel, skillMono.damageMultiplier);

        StartCoroutine(WaitToAnotherObject(counter, skill.spawnDelay));
    }
    private void Update()
    {
        if (!skill.isPassive && skillCD <= 0)
        {
            if (PlayerManager.instance.isAuto && abilityId == 0)
            {
                Spawn((int)skill.countObjects);
                skillCD = Mathf.Max(skill.stepMax, 0.2f);
            }
            else if(Input.GetKey(keyCode))
            {
                //CineMachineCameraShake.instance.Shake(10, .1f);
                Spawn((int)skill.countObjects);
                skillCD = Mathf.Max(skill.stepMax, 0.2f);
            }
        }
        else if (skill.isPassive && skill.countObjects > 0)
        {
            Spawn((int)skill.countObjects--);
        }
    }
    
}
