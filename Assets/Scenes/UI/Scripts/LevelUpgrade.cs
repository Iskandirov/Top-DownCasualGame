using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpgrade : MonoBehaviour
{
    [Header("Game Objects")]
    public Expirience Exp;
    public GameObject starObj;

    ActivateAbilities abilities;

    [Header("Description text buttons")]
    public TextMeshProUGUI firstBuff;
    public TextMeshProUGUI secondBuff;

    [Header("Image buttons")]
    public Image firstBuffBtn;
    public Image secondBuffBtn;

    [Header("All variations of skill")]
    public List<bool> isSkillIcons;
    public int SkillIconsMax;
    public List<SavedSkillsData> skillsSaveTree;
    public List<SavedSkillsData> skillsSave;
    public List<SavedSkillsData> skillsLoad;
    public SavedSkillsData gold;
    public List<string> skillsUpdatedList;

    [Header("Two random skill count")]
    public List<int> choise = new List<int>();

    [Header("Checkers")]
    public bool isOpenToUpgrade;
    public bool isFirstToUpgrade;
    public SetLanguage lang;
    public List<int> idList;
    public List<GameObject> CDSkillsObject;

    TagText objTextOne;
    TagText objTextTwo;
    Shoot objShoot;
    SkillCDLink objLinkSpell;
    ElementsCoeficients cef;
    // Start is called before the first frame update
    private void Start()
    {
        
        objShoot = GetComponentInParent<Shoot>();
        objLinkSpell = FindObjectOfType<SkillCDLink>();
        cef = FindObjectOfType<ElementsCoeficients>();
        SkillIconsMax = isSkillIcons.Count;
    }
    void OnEnable()
    {
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        if (!File.Exists(path))
        {
            SaveSkill();
        }

        LoadSkill(skillsLoad);

        //Initiation objects
        abilities = FindObjectOfType<ActivateAbilities>();

        //Random two abilitis ID
        RandomAbil();

    }
    //Random two abilitis
    public void RandomAbil()
    {
        objTextOne = firstBuff.GetComponent<TagText>();
        objTextTwo = secondBuff.GetComponent<TagText>();

        skillsLoad.Clear();
        LoadSkill(skillsLoad);


        List<GameObject> list = new List<GameObject>();

        if (skillsLoad.Count > 1)
        {
            // Оновити idList після видалення елементів
            idList.Clear();

            // Заповнити список ID з об'єктів
            foreach (SavedSkillsData obj in skillsLoad)
            {
                idList.Add(obj.ID);
            }

            while (choise.Count < 2)
            {
                int randomObject = skillsSave[Random.Range(0, skillsSave.Count)].ID;
                if (skillsLoad.Any(skill => skill.ID == randomObject) && !choise.Contains(randomObject))
                {
                    choise.Add(randomObject);
                }
            }

            // Оновити idList після видалення елементів
            idList.Clear();

            // Заповнити список ID з об'єктів
            foreach (SavedSkillsData obj in skillsLoad)
            {
                idList.Add(obj.ID);
            }

            firstBuff.text = skillsLoad.First(skill => skill.ID == choise[0]).Description[skillsLoad.First(skill => skill.ID == choise[0]).level];
            firstBuffBtn.sprite = Resources.Load<Sprite>(skillsLoad.First(skill => skill.ID == choise[0]).Name);
            objTextOne.tagText = skillsLoad.First(skill => skill.ID == choise[0]).tag[skillsLoad.First(skill => skill.ID == choise[0]).level];
            list.Add(firstBuff.gameObject);

            secondBuff.text = skillsLoad.First(skill => skill.ID == choise[1]).Description[skillsLoad.First(skill => skill.ID == choise[1]).level];
            secondBuffBtn.sprite = Resources.Load<Sprite>(skillsLoad.First(skill => skill.ID == choise[1]).Name);
            objTextTwo.tagText = skillsLoad.First(skill => skill.ID == choise[1]).tag[skillsLoad.First(skill => skill.ID == choise[1]).level];
            list.Add(secondBuff.gameObject);
        }
        else if (skillsLoad.Count == 1)
        {
            // Оновити idList після видалення елементів
            idList.Clear();

            choise.Add(skillsLoad[0].ID);

            firstBuff.text = skillsLoad[0].Description[skillsLoad[0].level];
            firstBuffBtn.sprite = Resources.Load<Sprite>(skillsLoad[0].Name);
            objTextOne.tagText = skillsLoad[0].tag[skillsLoad[0].level];
            list.Add(firstBuff.gameObject);

            choise.Add(999);

            secondBuff.text = gold.Description[0];
            secondBuffBtn.sprite = Resources.Load<Sprite>(gold.Name);
            objTextTwo.tagText = gold.tag[0];
            list.Add(secondBuff.gameObject);
        }
        else if (skillsLoad.Count == 0)
        {
            choise.Add(999);

            firstBuff.text = gold.Description[0];
            firstBuffBtn.sprite = Resources.Load<Sprite>(gold.Name);
            objTextOne.tagText = gold.tag[0];
            list.Add(firstBuff.gameObject);

            choise.Add(999);

            secondBuff.text = gold.Description[0];
            secondBuffBtn.sprite = Resources.Load<Sprite>(gold.Name);
            objTextTwo.tagText = gold.tag[0];
            list.Add(secondBuff.gameObject);
        }

        lang.settings.UpdateText(list);
    }

    //Choose first button
    public void UpgradeFirst()
    {
        isFirstToUpgrade = true;
        Udgrade(choise[0]);
        choise.Clear();
    }
    //Choose second button
    public void UpgradeSecond()
    {
        isFirstToUpgrade = false;
        Udgrade(choise[1]);
        choise.Clear();
    }
    public void Udgrade(int skillPoint)
    {
        ActivateAbilities abil = abilities;
        if (skillPoint != 999)
        {
            for (int i = 0; i < abil.countActiveAbilities; i++)
            {
                if (abil.abilities[i].sprite == Resources.Load<Sprite>(skillsSave[skillPoint].Name))
                {
                    Instantiate(starObj, abil.abilitiesObj[i].GetComponentInChildren<HorizontalLayoutGroup>().transform);

                    if (skillPoint == 0)
                    {
                        //Updates for bullet
                        if (skillsLoad[skillPoint].level == 1)
                        {
                            objShoot.isLevelTwo = true;
                            objShoot.secondBulletCount = (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            objShoot.attackSpeed -= skillsSave[skillPoint].stat1[skillsSave[skillPoint].level] / 100;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            objShoot.secondBulletCount = (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            objShoot.isLevelFive = true;
                        }
                    }
                    else if (skillPoint == 1)
                    {
                        //Updates for lightning
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<Lightning>().maxEnemiesToShoot += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Electricity += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<Lightning>().damage *= (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Electricity += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<Lightning>().stunTime += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Electricity += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<Lightning>().stepMax -= (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Electricity += 0.1f;
                        }
                    }
                    else if (skillPoint == 2)
                    {
                        //Updates for chair
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<SpawnBlood>().radius += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Water += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<SpawnBlood>().damage += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Water += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<SpawnBlood>().numOfChair += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Water += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<SpawnBlood>().damageTickMax -= skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Water += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                    }
                    else if (skillPoint == 3)
                    {
                        //Updates for pill
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<LightOn>().stepGhostMax += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Grass += 0.1f;
                            cef.Wind += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<LightOn>().attackSpeedBuff += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Grass += 0.1f;
                            cef.Wind += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<LightOn>().moveSpeedBuff += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Grass += 0.1f;
                            cef.Wind += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<LightOn>().dashTime = skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Grass += 0.1f;
                            cef.Wind += 0.1f;
                        }
                    }
                    else if (skillPoint == 4)
                    {
                        //Updates for fire wave
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<FireWaveSpawner>().damage += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Fire += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<FireWaveSpawner>().isLevelThree = true;
                            cef.Fire += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<FireWaveSpawner>().stepMax -= (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Fire += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<FireWaveSpawner>().burnDamage = (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Fire += 0.1f;
                        }
                    }
                    else if (skillPoint == 5)
                    {
                        //Updates for heal
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<HealSpawner>().heal += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Grass += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<HealSpawner>().isLevelTwo = true;
                            cef.Grass += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<HealSpawner>().stepMax -= (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Grass += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<HealSpawner>().heal += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Grass += 0.1f;
                        }
                    }
                    else if (skillPoint == 6)
                    {
                        //Updates for reload
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ReloadSkillSpawner>().count += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ReloadSkillSpawner>().stepMax -= (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ReloadSkillSpawner>().count += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ReloadSkillSpawner>().isLevelFive = true;
                        }
                    }
                    else if (skillPoint == 7)
                    {
                        //Updates for impuls
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ImpulsSpawner>().powerGrow += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Wind += 0.1f;
                            cef.Grass += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ImpulsSpawner>().stepMax -= (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Wind += 0.1f;
                            cef.Grass += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ImpulsSpawner>().isFour = true;
                            cef.Wind += 0.1f;
                            cef.Grass += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ImpulsSpawner>().isFive = true;
                            cef.Wind += 0.1f;
                            cef.Grass += 0.1f;
                        }
                    }
                    else if (skillPoint == 8)
                    {
                        //Updates for shield
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ShieldSpawner>().ShieldHP += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ShieldSpawner>().isThree = true;
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ShieldSpawner>().isFour = true;
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<ShieldSpawner>().isFive = true;
                            cef.Dirt += 0.1f;
                        }
                    }
                    else if (skillPoint == 9)
                    {
                        //Updates for beam
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<BeamSpawner>().isTwo = true;
                            cef.Steam += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<BeamSpawner>().damage += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Steam += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<BeamSpawner>().lifeTime += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Steam += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<BeamSpawner>().stepMax -= (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Steam += 0.1f;
                        }
                    }
                    else if (skillPoint == 10)
                    {
                        //Updates for tower
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<TowerSpawner>().lifeTime += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Water += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<TowerSpawner>().isThree = true;
                            cef.Water += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<TowerSpawner>().attackSpeed -= skillsSave[skillPoint].stat1[skillsSave[skillPoint].level] / 100;
                            cef.Water += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<TowerSpawner>().isFive = true;
                            cef.Water += 0.1f;
                        }
                    }
                    else if (skillPoint == 11)
                    {
                        //Updates for summon
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<SummonerEnemy>().lifeTime += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<SummonerEnemy>().isThree = true;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<SummonerEnemy>().attackSpeed -= skillsSave[skillPoint].stat1[skillsSave[skillPoint].level] / 100;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<SummonerEnemy>().isFive = true;
                        }
                    }
                    else if (skillPoint == 12)
                    {
                        //Updates for meteor
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<MeteorSpawner>().damage += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                            cef.Fire += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<MeteorSpawner>().isThree = true;
                            cef.Fire += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<MeteorSpawner>().isFour = true;
                            cef.Fire += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<MeteorSpawner>().isFive = true;
                            cef.Fire += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                    }
                    else if (skillPoint == 13)
                    {
                        //Updates for illusion
                        if (skillsSave[skillPoint].level == 1)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<IllusionSpawner>().isTwo = true;
                        }
                        else if (skillsSave[skillPoint].level == 2)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<IllusionSpawner>().lifeTime += (int)skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
                        }
                        else if (skillsSave[skillPoint].level == 3)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<IllusionSpawner>().isFour = true;
                        }
                        else if (skillsSave[skillPoint].level == 4)
                        {
                            skillsSave[skillPoint].skillObj.GetComponent<IllusionSpawner>().isFive = true;
                        }
                    }

                    skillsSave[skillPoint].level += 1;
                    ModifyJsonField(skillsSave[skillPoint], skillsSave[skillPoint].level);
                }
            }
            if (skillsSave[skillPoint].level == 0)
            {
                skillsSave[skillPoint].level += 1;
                ModifyJsonField(skillsSave[skillPoint], skillsSave[skillPoint].level);
                SetActiveAbil(abil, skillPoint);
                objLinkSpell.valuesList.Add(0);
                if (skillsSave[skillPoint].isPassive == false)
                {
                    isSkillIcons[skillPoint] = true;
                    skillsSave[skillPoint].skillObj.GetComponent<CDSkillObject>().CD = skillsSave[skillPoint].CD;
                    skillsSave[skillPoint].skillObj.GetComponentInParent<CDSkillObject>().number = abil.countActiveAbilities - 1;
                    abilities.abilitiesObj[abil.countActiveAbilities - 1].GetComponent<CDSkills>().number = abil.countActiveAbilities - 1;
                }
                else if (skillsSave[skillPoint].isPassive && skillPoint != 999)
                {
                    isSkillIcons[skillPoint] = true;
                    abilities.abilitiesObj[abil.countActiveAbilities - 1].GetComponent<CDSkills>().number = abil.countActiveAbilities - 1;
                    abilities.abilitiesObj[abil.countActiveAbilities - 1].GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
                }
                if (abilities.countActiveAbilities == abilities.abilitiesObj.Length && isSkillIcons.Count == SkillIconsMax)
                {
                    RemoveAllOtherLines();
                }
            }
            if (skillsSave[skillPoint].level == 5)
            {
                RemoveLine(skillPoint);
            }
        }
        else if (skillPoint == 999)
        {
            FindObjectOfType<KillCount>().score += 20;
        }


        objLinkSpell.Check();

        SkillIconsMax = skillsLoad.Count;
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    //Remove ability ID
    public void RemoveLine(int SkillPoint)
    {
        RemoveKeyFromJSONFile(SkillPoint, 1);
        skillsLoad.Clear();
        LoadSkill(skillsLoad);
    }
    //Activate skill if it`s not active
    public void SetActiveAbil(ActivateAbilities abil, int skillPoint)
    {
        if (skillsSave[skillPoint].isPassive == false)
        {
            SetActiviti(skillsSave[skillPoint].skillObj, true);
        }
        else
        {
            if (skillPoint == 0)
            {
                objShoot.damageToGive += skillsSave[skillPoint].stat1[skillsSave[skillPoint].level];
            }
        }
        abil.abilitiesObj[abil.countActiveAbilities].SetActive(true);
        abil.abilities[abil.countActiveAbilities].sprite = Resources.Load<Sprite>(skillsSave[skillPoint].Name);
        abil.countActiveAbilities += 1;


    }
    //Remove lines from massive if skill have max value of stars
    public void RemoveAllOtherLines()
    {
        for (int i = 0; i < skillsLoad.Count; i++)
        {
            if (isSkillIcons[i] == false)
            {
                RemoveKeyFromJSONFile(i,0);
            }
        }
        skillsLoad.Clear();
        LoadSkill(skillsLoad);
        
    }

    public void LoadSkill(List<SavedSkillsData> itemsRead)
    {
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            // Перебір кожного запису і заміна шляху до зображення на зображення зі списку sprites
            foreach (string jsonLine in lines)
            {
                SavedSkillsData data = JsonUtility.FromJson<SavedSkillsData>(jsonLine);

                data.Image = Resources.Load<Sprite>(data.Name);

                itemsRead.Add(data);
            }
        }
        else
        {
            File.Create(path);
        }
    }

    public void SetActiviti(GameObject skill, bool activate)
    {
        skill.SetActive(activate);
    }
    private void SaveSkill()
    {
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        StreamWriter writer = new StreamWriter(path, true);

        SavedSkillsData data = new SavedSkillsData();
        foreach (SavedSkillsData item in skillsSave)
        {
            data.Name = item.Name;
            data.ID = item.ID;
            data.level = item.level;
            data.Description = item.Description;
            data.stat1 = item.stat1;
            data.tag = item.tag;
            data.tagRare = item.tagRare;
            data.skillObj = item.skillObj;
            data.isPassive = item.isPassive;
            data.CD = item.CD;

            string jsonData = JsonUtility.ToJson(data);
            writer.WriteLine(jsonData);
        }
        writer.Close();

    }
    private void RemoveKeyFromJSONFile(int IDObj, int minus)
    {
        skillsUpdatedList.Clear();
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        if (File.Exists(path))
        {
            string[] jsonLines = File.ReadAllLines(path);
            foreach (var jsonLine in jsonLines)
            {

                SavedSkillsData data = JsonUtility.FromJson<SavedSkillsData>(jsonLine);
                if (data.tag[data.level - minus] != skillsSave[IDObj].tag[skillsSave[IDObj].level - minus])
                {
                    skillsUpdatedList.Add(jsonLine);
                }
            }
            // Записуємо оновлений масив рядків в файл після завершення циклу foreach
            File.WriteAllLines(path, skillsUpdatedList.ToArray());
        }
    }
    public void ModifyJsonField(SavedSkillsData IDObj, int newValue)
    {
        skillsUpdatedList.Clear();
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        if (File.Exists(path))
        {
            string[] jsonLines = File.ReadAllLines(path);
            foreach (var jsonLine in jsonLines)
            {
                SavedSkillsData data = JsonUtility.FromJson<SavedSkillsData>(jsonLine);
                if (data.ID != IDObj.ID)
                {
                    skillsUpdatedList.Add(jsonLine);
                }
                else
                {
                    data.level = newValue;
                    string updatedJsonLine = JsonUtility.ToJson(data);
                    skillsUpdatedList.Add(updatedJsonLine);
                }
            }
            File.WriteAllText(path, string.Empty);
            // Записуємо оновлений масив рядків в файл після завершення циклу foreach
            File.WriteAllLines(path, skillsUpdatedList.ToArray());
        }
    }
}

