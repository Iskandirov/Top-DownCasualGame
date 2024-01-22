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

    KeyCode keyCode;
    TextMeshProUGUI cDText;
    // Update is called once per frame
    void Start()
    {
        skill.skill = this;
        skillMono.Set(skill,number);
        cDText = text.GetComponent<TextMeshProUGUI>();
        StartCoroutine(SetBumberToSkill());
    }
    void FixedUpdate()
    {
        skillCD -= Time.fixedDeltaTime;
        if (skillCD <= 0)
        {
            text.SetActive(false);
        }
        else
        {
            text.SetActive(true);
        }
        cDText.text = skillCD.ToString("0");
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
        keyCode = (KeyCode)((int)KeyCode.Alpha0 + number);
    }
    private IEnumerator WaitToAnotherObject(int count,float delay)
    {
        for (int i = 0; i < count - 1; i++)
        {
            yield return new WaitForSeconds(delay);
            skillMono.CreateSpellByType(type, skill.objToSpawn, skillMono, currentStatLevel);
        }
    }
    void Spawn(int counter)
    {
        skillMono.CreateSpellByType(type, skill.objToSpawn, skillMono, currentStatLevel);

        StartCoroutine(WaitToAnotherObject(counter, skill.spawnDelay));
    }
    private void Update()
    {
        spriteCD.fillAmount = skillCD / skill.stepMax;
        if (number != 0 && !skill.isPassive)
        {
            if (skillCD <= 0 && Input.GetKeyDown(keyCode))
            {
                Spawn((int)skill.countObjects);
                skillCD = skill.stepMax;
            }
        }
        else if (skillCD <= 0 && Input.GetMouseButton(0) && number == 0 && !skill.isPassive)
        {
            Spawn(PlayerManager.instance.secondBulletCount);
            skillCD = skill.stepMax;
        }
        else if (skill.isPassive && skill.countObjects > 0)
        {
            Spawn((int)skill.countObjects--);
        }
    }
}
