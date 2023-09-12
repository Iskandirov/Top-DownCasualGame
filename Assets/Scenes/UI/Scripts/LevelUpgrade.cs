using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Mathematics;
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
    //public List<SavedSkillsData> skillsSaveTree;
    public List<SavedSkillsData> skillsSave;
    //public List<SavedSkillsData> skillsLoad;
    public SavedSkillsData gold;
    public List<string> skillsUpdatedList;

    [Header("Two random skill count")]
    public List<int> choise = new List<int>();

    [Header("Checkers")]
    public bool isOpenToUpgrade;
    public bool isFirstToUpgrade;
    public SetLanguage lang;
    public List<GameObject> CDSkillsObject;

    TagText objTextOne;
    TagText objTextTwo;
    Shoot objShoot;
    SkillCDLink objLinkSpell;
    ElementsCoeficients cef;
    SavedSkillsData resultId;

    Move PanelChecker;
    // Start is called before the first frame update
    private void Start()
    {
        PanelChecker = FindObjectOfType<Move>();
        resultId = new SavedSkillsData();
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

        LoadSkill(skillsSave);

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

        skillsSave.Clear();
        LoadSkill(skillsSave);


        List<GameObject> list = new List<GameObject>();

        if (skillsSave.Count > 1)
        {
            while (choise.Count < 2)
            {
                int randomObject = skillsSave[UnityEngine.Random.Range(0, skillsSave.Count)].ID;
                if (skillsSave.Any(skill => skill.ID == randomObject) && !choise.Contains(randomObject))
                {
                    choise.Add(randomObject);
                }
            }

            firstBuff.text = skillsSave.First(skill => skill.ID == choise[0]).Description[skillsSave.First(skill => skill.ID == choise[0]).level];
            firstBuffBtn.sprite = Resources.Load<Sprite>(skillsSave.First(skill => skill.ID == choise[0]).Name);
            objTextOne.tagText = skillsSave.First(skill => skill.ID == choise[0]).tag[skillsSave.First(skill => skill.ID == choise[0]).level];
            list.Add(firstBuff.gameObject);

            secondBuff.text = skillsSave.First(skill => skill.ID == choise[1]).Description[skillsSave.First(skill => skill.ID == choise[1]).level];
            secondBuffBtn.sprite = Resources.Load<Sprite>(skillsSave.First(skill => skill.ID == choise[1]).Name);
            objTextTwo.tagText = skillsSave.First(skill => skill.ID == choise[1]).tag[skillsSave.First(skill => skill.ID == choise[1]).level];
            list.Add(secondBuff.gameObject);
        }
        else if (skillsSave.Count == 1)
        {

            choise.Add(skillsSave[0].ID);

            firstBuff.text = skillsSave[0].Description[skillsSave[0].level];
            firstBuffBtn.sprite = Resources.Load<Sprite>(skillsSave[0].Name);
            objTextOne.tagText = skillsSave[0].tag[skillsSave[0].level];
            list.Add(firstBuff.gameObject);

            choise.Add(999);

            secondBuff.text = gold.Description[0];
            secondBuffBtn.sprite = Resources.Load<Sprite>(gold.Name);
            objTextTwo.tagText = gold.tag[0];
            list.Add(secondBuff.gameObject);
        }
        else if (skillsSave.Count == 0)
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
        for (int i = 0;i < skillsSave.Count;i++)
        {
            if (skillsSave[i].ID == skillPoint)
            {
                resultId = skillsSave[i];
            }
        }
        ActivateAbilities abil = abilities;
        if (skillPoint != 999)
        {
            for (int i = 0; i < abil.countActiveAbilities; i++)
            {
                if (abil.transform.GetChild(i).GetComponent<CDSkills>().abilityId == skillPoint)
                {
                    Instantiate(starObj, abil.abilitiesObj[i].GetComponentInChildren<HorizontalLayoutGroup>().transform);

                    if (skillPoint == 0)
                    {
                        //Updates for bullet
                        if (resultId.level == 1)
                        {
                            objShoot.isLevelTwo = true;
                            objShoot.secondBulletCount = (int)resultId.stat1[resultId.level];
                        }
                        else if (resultId.level == 2)
                        {
                            objShoot.attackSpeed -= resultId.stat1[resultId.level] / 100;
                        }
                        else if (resultId.level == 3)
                        {
                            objShoot.secondBulletCount = (int)resultId.stat1[resultId.level];
                        }
                        else if (resultId.level == 4)
                        {
                            objShoot.isLevelFive = true;
                        }
                    }
                    else if (skillPoint == 1)
                    {
                        //Updates for lightning
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<Lightning>().maxEnemiesToShoot += (int)resultId.stat1[resultId.level];
                            cef.Electricity += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<Lightning>().damage *= (int)resultId.stat1[resultId.level];
                            cef.Electricity += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<Lightning>().stunTime += (int)resultId.stat1[resultId.level];
                            cef.Electricity += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<Lightning>().stepMax -= (int)resultId.stat1[resultId.level];
                            cef.Electricity += 0.1f;
                        }
                    }
                    else if (skillPoint == 2)
                    {
                        //Updates for chair
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<SpawnBlood>().radius += (int)resultId.stat1[resultId.level];
                            cef.Water += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<SpawnBlood>().damage += (int)resultId.stat1[resultId.level];
                            cef.Water += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<SpawnBlood>().numOfChair += (int)resultId.stat1[resultId.level];
                            cef.Water += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<SpawnBlood>().damageTickMax -= resultId.stat1[resultId.level];
                            cef.Water += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                    }
                    else if (skillPoint == 3)
                    {
                        //Updates for pill
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<LightOn>().stepGhostMax += (int)resultId.stat1[resultId.level];
                            cef.Grass += 0.1f;
                            cef.Wind += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<LightOn>().attackSpeedBuff += (int)resultId.stat1[resultId.level];
                            cef.Grass += 0.1f;
                            cef.Wind += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<LightOn>().moveSpeedBuff += (int)resultId.stat1[resultId.level];
                            cef.Grass += 0.1f;
                            cef.Wind += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<LightOn>().dashTime = resultId.stat1[resultId.level];
                            cef.Grass += 0.1f;
                            cef.Wind += 0.1f;
                        }
                    }
                    else if (skillPoint == 4)
                    {
                        //Updates for fire wave
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<FireWaveSpawner>().damage += (int)resultId.stat1[resultId.level];
                            cef.Fire += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<FireWaveSpawner>().isLevelThree = true;
                            cef.Fire += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<FireWaveSpawner>().stepMax -= (int)resultId.stat1[resultId.level];
                            cef.Fire += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<FireWaveSpawner>().burnDamage = (int)resultId.stat1[resultId.level];
                            cef.Fire += 0.1f;
                        }
                    }
                    else if (skillPoint == 5)
                    {
                        //Updates for heal
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<HealSpawner>().heal += (int)resultId.stat1[resultId.level];
                            cef.Grass += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<HealSpawner>().isLevelTwo = true;
                            cef.Grass += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<HealSpawner>().stepMax -= (int)resultId.stat1[resultId.level];
                            cef.Grass += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<HealSpawner>().heal += (int)resultId.stat1[resultId.level];
                            cef.Grass += 0.1f;
                        }
                    }
                    else if (skillPoint == 6)
                    {
                        //Updates for reload
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<ReloadSkillSpawner>().count += (int)resultId.stat1[resultId.level];
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<ReloadSkillSpawner>().stepMax -= (int)resultId.stat1[resultId.level];
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<ReloadSkillSpawner>().count += (int)resultId.stat1[resultId.level];
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<ReloadSkillSpawner>().isLevelFive = true;
                        }
                    }
                    else if (skillPoint == 7)
                    {
                        //Updates for impuls
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<ImpulsSpawner>().powerGrow += (int)resultId.stat1[resultId.level];
                            cef.Wind += 0.1f;
                            cef.Grass += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<ImpulsSpawner>().stepMax -= (int)resultId.stat1[resultId.level];
                            cef.Wind += 0.1f;
                            cef.Grass += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<ImpulsSpawner>().isFour = true;
                            cef.Wind += 0.1f;
                            cef.Grass += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<ImpulsSpawner>().isFive = true;
                            cef.Wind += 0.1f;
                            cef.Grass += 0.1f;
                        }
                    }
                    else if (skillPoint == 8)
                    {
                        //Updates for shield
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<ShieldSpawner>().ShieldHP += (int)resultId.stat1[resultId.level];
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<ShieldSpawner>().isThree = true;
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<ShieldSpawner>().isFour = true;
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<ShieldSpawner>().isFive = true;
                            cef.Dirt += 0.1f;
                        }
                    }
                    else if (skillPoint == 9)
                    {
                        //Updates for beam
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<BeamSpawner>().isTwo = true;
                            cef.Steam += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<BeamSpawner>().damage += (int)resultId.stat1[resultId.level];
                            cef.Steam += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<BeamSpawner>().lifeTime += (int)resultId.stat1[resultId.level];
                            cef.Steam += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<BeamSpawner>().stepMax -= (int)resultId.stat1[resultId.level];
                            cef.Steam += 0.1f;
                        }
                    }
                    else if (skillPoint == 10)
                    {
                        //Updates for tower
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<TowerSpawner>().lifeTime += (int)resultId.stat1[resultId.level];
                            cef.Water += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<TowerSpawner>().isThree = true;
                            cef.Water += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<TowerSpawner>().attackSpeed -= resultId.stat1[resultId.level] / 100;
                            cef.Water += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<TowerSpawner>().isFive = true;
                            cef.Water += 0.1f;
                        }
                    }
                    else if (skillPoint == 11)
                    {
                        //Updates for summon
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<SummonerEnemy>().lifeTime += (int)resultId.stat1[resultId.level];
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<SummonerEnemy>().isThree = true;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<SummonerEnemy>().attackSpeed -= resultId.stat1[resultId.level] / 100;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<SummonerEnemy>().isFive = true;
                        }
                    }
                    else if (skillPoint == 12)
                    {
                        //Updates for meteor
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<MeteorSpawner>().damage += (int)resultId.stat1[resultId.level];
                            cef.Fire += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<MeteorSpawner>().isThree = true;
                            cef.Fire += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<MeteorSpawner>().isFour = true;
                            cef.Fire += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<MeteorSpawner>().isFive = true;
                            cef.Fire += 0.1f;
                            cef.Dirt += 0.1f;
                        }
                    }
                    else if (skillPoint == 13)
                    {
                        //Updates for illusion
                        if (resultId.level == 1)
                        {
                            resultId.skillObj.GetComponent<IllusionSpawner>().isTwo = true;
                        }
                        else if (resultId.level == 2)
                        {
                            resultId.skillObj.GetComponent<IllusionSpawner>().lifeTime += (int)resultId.stat1[resultId.level];
                        }
                        else if (resultId.level == 3)
                        {
                            resultId.skillObj.GetComponent<IllusionSpawner>().isFour = true;
                        }
                        else if (resultId.level == 4)
                        {
                            resultId.skillObj.GetComponent<IllusionSpawner>().isFive = true;
                        }
                    }

                    resultId.level += 1;
                    ModifyJsonField(resultId, resultId.level);
                }
            }
            if (resultId.level == 0)
            {
                resultId.level += 1;
                ModifyJsonField(resultId, resultId.level);
                SetActiveAbil(abil, skillPoint);
                objLinkSpell.valuesList.Add(0);
                if (resultId.isPassive == false)
                {
                    isSkillIcons[skillPoint] = true;
                    resultId.skillObj.GetComponent<CDSkillObject>().CD = resultId.CD;
                    resultId.skillObj.GetComponentInParent<CDSkillObject>().number = abil.countActiveAbilities - 1;
                    abilities.abilitiesObj[abil.countActiveAbilities - 1].GetComponent<CDSkills>().number = abil.countActiveAbilities - 1;
                }
                else if (resultId.isPassive && skillPoint != 999)
                {
                    isSkillIcons[skillPoint] = true;
                    abilities.abilitiesObj[abil.countActiveAbilities - 1].GetComponent<CDSkills>().number = abil.countActiveAbilities - 1;
                    abilities.abilitiesObj[abil.countActiveAbilities - 1].GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
                }
               
            }
            RemoveLine(skillPoint);

            if (abilities.countActiveAbilities == abilities.abilitiesObj.Length)
            {
                RemoveAllOtherLines();
            }
        }
        else if (skillPoint == 999)
        {
            FindObjectOfType<KillCount>().score += 20;
        }


        objLinkSpell.Check();

        SkillIconsMax = skillsSave.Count;
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        PanelChecker.otherPanelOpened = false;
    }
    //Remove ability ID
    public void RemoveLine(int skillPoint)
    {
        if (resultId.level == 5)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < skillsSave.Count; i++)
            {
                if (skillsSave[i].ID == skillPoint)
                {
                    indexesToRemove.Add(i);
                }
            }

            // Видаляємо елементи за збереженими індексами в зворотньому порядку
            for (int i = indexesToRemove.Count - 1; i >= 0; i--)
            {
                int indexToRemove = indexesToRemove[i];
                RemoveKeyFromJSONFile(indexToRemove);
                isSkillIcons.RemoveAt(indexToRemove);
            }

        }
        skillsSave.Clear();
        LoadSkill(skillsSave);
    }
    //Activate skill if it`s not active
    public void SetActiveAbil(ActivateAbilities abil, int skillPoint)
    {
        if (resultId.isPassive == false)
        {
            SetActiviti(resultId.skillObj, true);
        }
        else
        {
            if (skillPoint == 0)
            {
                objShoot.damageToGive += resultId.stat1[resultId.level];
            }
        }
        abil.abilitiesObj[abil.countActiveAbilities].SetActive(true);
        abil.abilities[abil.countActiveAbilities].sprite = Resources.Load<Sprite>(resultId.Name);
        abil.countActiveAbilities += 1;
        abil.transform.GetChild(abil.countActiveAbilities-1).GetComponent<CDSkills>().abilityId = skillPoint;


    }
    public void RemoveAllOtherLines()
    {
        for (int i = 0; i < skillsSave.Count; i++)
        {
            if (isSkillIcons[i] == false)
            {
                RemoveKeyFromJSONFile(i);
            }
        }
        int y = 0;

        while (y < isSkillIcons.Count)
        {
            if (isSkillIcons[y] == false)
            {
                isSkillIcons.RemoveAt(y);
            }
            else
            {
                y++;
            }
        }
        skillsSave.Clear();
        LoadSkill(skillsSave);
    }

    public void LoadSkill(List<SavedSkillsData> itemsRead)
    {
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

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
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            foreach (SavedSkillsData item in skillsSave)
            {
                SavedSkillsData data = new SavedSkillsData();
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

            
    }

    private void RemoveKeyFromJSONFile(int IDObj)
    {
        skillsUpdatedList.Clear();
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        if (File.Exists(path))
        {
            string[] jsonLines = File.ReadAllLines(path);
            for (int i = 0; i < jsonLines.Length; i++)
            {
                SavedSkillsData data = JsonUtility.FromJson<SavedSkillsData>(jsonLines[i]);
                if (data.ID != skillsSave[IDObj].ID)
                {
                    skillsUpdatedList.Add(jsonLines[i]);
                }
            }
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
            File.WriteAllLines(path, skillsUpdatedList.ToArray());
        }
    }

}

