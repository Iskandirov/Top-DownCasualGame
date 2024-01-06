using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpgrade : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject starObj;


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
    public int[] choose;

    [Header("Checkers")]
    public bool isOpenToUpgrade;
    public bool isFirstToUpgrade;
    public List<GameObject> CDSkillsObject;

    TagText objTextOne;
    TagText objTextTwo;
    SkillCDLink objLinkSpell;
    public SavedSkillsData resultId;

    PlayerManager player;
    GameManager gameManager;
    public GameObject abil;
    bool empty;
    public static LevelUpgrade instance;
   
    private void Awake()
    {
        instance ??= this;
    }
    // Start is called before the first frame update
    private void Start()
    {
        player = PlayerManager.instance;
        gameManager = GameManager.Instance;
        resultId = new SavedSkillsData();
        objLinkSpell = FindObjectOfType<SkillCDLink>();
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

        choose = skillsSave.Count > 1
            ? Enumerable.Range(0, 2)
                .Select(_ => skillsSave[UnityEngine.Random.Range(0, skillsSave.Count)].ID)
                .Distinct()
                .ToArray()
            : Enumerable.Repeat(999, 2).ToArray();

        for (int i = 0; i < 2; i++)
        {
            var skill = i < skillsSave.Count
                ? skillsSave.First(s => s.ID == choose[i])
                : gold;
            var buffText = i == 0 ? firstBuff : secondBuff;
            var buffBtn = i == 0 ? firstBuffBtn : secondBuffBtn;
            var objText = i == 0 ? objTextOne : objTextTwo;

            buffText.text = skill.Description[skill.level];
            buffBtn.sprite = Resources.Load<Sprite>(skill.Name);
            objText.tagText = skill.tag[skill.level];
            list.Add(buffText.gameObject);
        }

        GameManager.Instance.UpdateText(list);
    }

    //Choose first button
    public void UpgradeFirst()
    {
        isFirstToUpgrade = true;
        Udgrade(choose[0]);
    }
    //Choose second button
    public void UpgradeSecond()
    {
        isFirstToUpgrade = false;
        Udgrade(choose[1]);
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
        
        if (skillPoint != 999)
        {
            for (int i = 0; i < player.countActiveAbilities; i++)
            {
                var abilityScript = abil.transform.GetChild(i).GetComponent<CDSkills>();

                if (abilityScript.abilityId == skillPoint)
                {
                    Instantiate(starObj, abilityScript.GetComponentInChildren<HorizontalLayoutGroup>().transform);
                    switch (skillPoint) 
                    {
                        case 0:
                            switch (resultId.level)
                            {
                                case 1:
                                    player.isLevelTwo = true;
                                    player.secondBulletCount = (int)resultId.stat1[resultId.level];
                                    break;
                                case 2:
                                    player.attackSpeed -= resultId.stat1[resultId.level] / 100;
                                    break;
                                case 3:
                                    player.secondBulletCount = (int)resultId.stat1[resultId.level];
                                    break;
                                case 4:
                                    player.isLevelFive = true;
                                    break;
                            }
                            //Updates for bullet
                            break;
                        case 1:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<Lightning>().maxEnemiesToShoot += (int)resultId.stat1[resultId.level];
                                    player.Electricity += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<Lightning>().damage *= (float)resultId.stat1[resultId.level];
                                    player.Electricity += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<Lightning>().stunTime += (float)resultId.stat1[resultId.level];
                                    player.Electricity += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<Lightning>().stepMax -= (float)resultId.stat1[resultId.level];
                                    player.Electricity += 0.1f;
                                    break;
                            }
                            //Updates for lightning
                            break;
                        case 2:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<SpawnBlood>().radius += (float)resultId.stat1[resultId.level];
                                    player.Water += 0.1f;
                                    player.Dirt += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<SpawnBlood>().damage += (float)resultId.stat1[resultId.level];
                                    player.Water += 0.1f;
                                    player.Dirt += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<SpawnBlood>().numOfChair += (float)resultId.stat1[resultId.level];
                                    player.Water += 0.1f;
                                    player.Dirt += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<SpawnBlood>().damageTickMax -= (float)resultId.stat1[resultId.level];
                                    player.Water += 0.1f;
                                    player.Dirt += 0.1f;
                                    break;
                            }
                            //Updates for chair
                            break;
                        case 3:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<MeteorSpawner>().damage += (float)resultId.stat1[resultId.level];
                                    player.Fire += 0.1f;
                                    player.Dirt += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<MeteorSpawner>().isThree = true;
                                    player.Fire += 0.1f;
                                    player.Dirt += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<MeteorSpawner>().isFour = true;
                                    player.Fire += 0.1f;
                                    player.Dirt += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<MeteorSpawner>().isFive = true;
                                    player.Fire += 0.1f;
                                    player.Dirt += 0.1f;
                                    break;
                            }
                            //Updates for meteor
                            break;
                        case 4:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<FireWaveSpawner>().damage += (float)resultId.stat1[resultId.level];
                                    player.Fire += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<FireWaveSpawner>().isLevelThree = true;
                                    player.Fire += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<FireWaveSpawner>().stepMax -= (float)resultId.stat1[resultId.level];
                                    player.Fire += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<FireWaveSpawner>().burnDamage = (float)resultId.stat1[resultId.level];
                                    player.Fire += 0.1f;
                                    break;
                            }
                            //Updates for fire wave
                            break;
                        case 5:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<HealSpawner>().heal += (float)resultId.stat1[resultId.level];
                                    player.Grass += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<HealSpawner>().isLevelTwo = true;
                                    player.Grass += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<HealSpawner>().stepMax -= (float)resultId.stat1[resultId.level];
                                    player.Grass += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<HealSpawner>().heal += (float)resultId.stat1[resultId.level];
                                    player.Grass += 0.1f;
                                    break;
                            }
                            //Updates for heal
                            break;
                        case 6:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<ReloadSkillSpawner>().count += (int)resultId.stat1[resultId.level];
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<ReloadSkillSpawner>().stepMax -= resultId.stat1[resultId.level];
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<ReloadSkillSpawner>().count += (int)resultId.stat1[resultId.level];
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<ReloadSkillSpawner>().isLevelFive = true;
                                    break;
                            }
                            //Updates for reload
                            break;
                        case 7:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<ImpulsSpawner>().powerGrow += resultId.stat1[resultId.level];
                                    player.Wind += 0.1f;
                                    player.Grass += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<ImpulsSpawner>().stepMax -= resultId.stat1[resultId.level];
                                    player.Wind += 0.1f;
                                    player.Grass += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<ImpulsSpawner>().isFour = true;
                                    player.Wind += 0.1f;
                                    player.Grass += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<ImpulsSpawner>().isFive = true;
                                    player.Wind += 0.1f;
                                    player.Grass += 0.1f;
                                    break;
                            }
                            //Updates for impuls
                            break;
                        case 8:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<ShieldSpawner>().ShieldHP += resultId.stat1[resultId.level];
                                    player.Dirt += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<ShieldSpawner>().isThree = true;
                                    player.Dirt += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<ShieldSpawner>().isFour = true;
                                    player.Dirt += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<ShieldSpawner>().isFive = true;
                                    player.Dirt += 0.1f;
                                    break;
                            }
                            //Updates for shield
                            break;
                        case 9:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<BeamSpawner>().isTwo = true;
                                    player.Steam += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<BeamSpawner>().damage += resultId.stat1[resultId.level];
                                    player.Steam += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<BeamSpawner>().lifeTime += resultId.stat1[resultId.level];
                                    player.Steam += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<BeamSpawner>().stepMax -= resultId.stat1[resultId.level];
                                    player.Steam += 0.1f;
                                    break;
                            }
                            //Updates for beam
                            break;
                        case 10:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<TowerSpawner>().lifeTime += (float)resultId.stat1[resultId.level];
                                    player.Water += 0.1f;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<TowerSpawner>().isThree = true;
                                    player.Water += 0.1f;
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<TowerSpawner>().attackSpeed -= resultId.stat1[resultId.level] / 100;
                                    player.Water += 0.1f;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<TowerSpawner>().isFive = true;
                                    player.Water += 0.1f;
                                    break;
                            }
                            //Updates for tower
                            break;
                        case 11:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<IllusionSpawner>().isTwo = true;
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<IllusionSpawner>().lifeTime += resultId.stat1[resultId.level];
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<IllusionSpawner>().isFour = true;
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<IllusionSpawner>().isFive = true;
                                    break;
                            }
                            //Updates for illusion
                            break;
                        case 12:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<Trail>().trailRenderer.time += (float)resultId.stat1[resultId.level];
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<Trail>().damage += resultId.stat1[resultId.level];
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<Trail>().size += resultId.stat1[resultId.level];
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<Trail>().isFive = true;
                                    break;
                            }
                            //Updates for trail
                            break;
                        case 13:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<VortexSpawner>().lifeTime += resultId.stat1[resultId.level];
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<VortexSpawner>().radius += resultId.stat1[resultId.level];
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<VortexSpawner>().stepMax -= resultId.stat1[resultId.level];
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<VortexSpawner>().isFive = true;
                                    break;
                            }
                            //Updates for vortex
                            break;
                        case 14:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<IceWallSpawner>().lifeTime += resultId.stat1[resultId.level];
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<IceWallSpawner>().wide += resultId.stat1[resultId.level];
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<IceWallSpawner>().stepMax -= resultId.stat1[resultId.level];
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<IceWallSpawner>().damage += resultId.stat1[resultId.level];
                                    continue;
                            }
                            //Updates for icewall
                            break;
                        case 15:
                            switch (resultId.level)
                            {
                                case 1:
                                    resultId.skillObj.GetComponent<MagicAxeSpawner>().damage += resultId.stat1[resultId.level];
                                    break;
                                case 2:
                                    resultId.skillObj.GetComponent<MagicAxeSpawner>().radius += resultId.stat1[resultId.level];
                                    break;
                                case 3:
                                    resultId.skillObj.GetComponent<MagicAxeSpawner>().stepMax -= resultId.stat1[resultId.level];
                                    break;
                                case 4:
                                    resultId.skillObj.GetComponent<MagicAxeSpawner>().isFive = true;
                                    break;
                            }
                            //Updates for magic axe
                            break;
                    }
                    resultId.Upgrade();
                }
            }
            if (resultId.level == 0)
            {
                resultId.Upgrade();
                SetActiveAbil(player, abil, skillPoint);
                objLinkSpell.valuesList.Add(0);

                isSkillIcons[skillPoint] = true;
                resultId.skillObj.GetComponent<CDSkillObject>().CD = resultId.CD;
                resultId.skillObj.GetComponentInParent<CDSkillObject>().number = player.countActiveAbilities - 1;
                player.abilitiesObj[player.countActiveAbilities - 1].GetComponent<CDSkills>().number = player.countActiveAbilities - 1;

                if (resultId.isPassive == false)
                {
                    
                }
                else if (resultId.isPassive && skillPoint != 999)
                {
                    player.abilitiesObj[player.countActiveAbilities - 1].GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
                }
               
            }
            RemoveLine(skillPoint);

            if (player.countActiveAbilities == player.abilitiesObj.Length)
            {
                RemoveAllOtherLines();
            }
        }
        else if (skillPoint == 999)
        {
            gameManager.score += 20;
        }
        objLinkSpell.Check();

        SkillIconsMax = skillsSave.Count;
        //Time.timeScale = 1f;
        //gameObject.SetActive(false);
        gameManager.ClosePanel(gameManager.levelPanel);
        //PanelChecker.otherPanelOpened = false;
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
    public void SetActiveAbil(PlayerManager player,GameObject abilObj, int skillPoint)
    {
        SetActiviti(resultId.skillObj, true);
        if (skillPoint == 0)
        {
            player.damageToGive += resultId.stat1[resultId.level];
        }
        player.abilitiesObj[player.countActiveAbilities].SetActive(true);
        player.abilities[player.countActiveAbilities].sprite = Resources.Load<Sprite>(resultId.Name);
        player.countActiveAbilities += 1;
        abilObj.transform.GetChild(player.countActiveAbilities - 1).GetComponent<CDSkills>().abilityId = skillPoint;
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

