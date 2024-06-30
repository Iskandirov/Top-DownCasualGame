using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ElementsSprites
{
    public string skillName;
    public List<Elements.status> SkillElements;
}
[DefaultExecutionOrder(11)]
public class LevelUpgrade : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject starObj;

    [Header("Description text buttons")]
    public TextMeshProUGUI firstBuff;
    public TextMeshProUGUI secondBuff;
    public TextMeshProUGUI firstBuffReload;
    public TextMeshProUGUI secondBuffReload; 
    public TextMeshProUGUI firstBuffDamage;
    public TextMeshProUGUI secondBuffDamage; 
    public List<Image> firstBuffElements;
    public List<Image> secondBuffElements;
    public List<Color> colorBG;

    [Header("Image buttons")]
    public Image firstBuffBtn;
    public Image secondBuffBtn;

    [Header("All variations of skill")]
    public List<bool> isSkillIcons;
    public List<ElementsSprites> SkillElements;
    public int SkillIconsMax;
    public List<SavedSkillsData> skillsSave;
    public SavedSkillsData gold;
    public List<string> skillsUpdatedList;
    [Header("Two random skill count")]
    public List<int> choose;

    [Header("Checkers")]
    public bool isOpenToUpgrade;
    public bool isFirstToUpgrade;
    public List<GameObject> CDSkillsObject;

    TagText objTextOne;
    TagText objTextTwo;
    //SkillCDLink objLinkSpell;
    public SavedSkillsData resultId;

    PlayerManager player;
    GameManager gameManager;
    public GameObject abil;
    public static LevelUpgrade instance;
    private void Awake()
    {
        instance ??= this;
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        player = PlayerManager.instance;
        gameManager = GameManager.Instance;
        resultId = new SavedSkillsData();
        SkillIconsMax = isSkillIcons.Count;
        SetActiveAbil(player, abil, 0, true);
    }
    void OnEnable()
    {
        string path = Path.Combine(Application.persistentDataPath, "SkillData.txt");
        if (!File.Exists(path))
        {
            SaveSkill();
        }

        LoadSkill(skillsSave);

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
            ? GetTwoRandomNumbers(skillsSave)
            : new List<int> { 999, 999 };
        if (skillsSave.Count == 1)
        {
            choose[0] = skillsSave[0].ID;
        }
        for (int i = 0; i < 2 && i < choose.Count; i++)
        {
            var skill = i < skillsSave.Count
                ? skillsSave.FirstOrDefault(s => s.ID == choose[i])
                : gold;
            var buffText = i == 0 ? firstBuff : secondBuff;
            var buffCD = i == 0 ? firstBuffReload : secondBuffReload;
            var buffDMG = i == 0 ? firstBuffDamage : secondBuffDamage;
            var buffElementImg = i == 0 ? firstBuffElements : secondBuffElements;
            var buffBtn = i == 0 ? firstBuffBtn : secondBuffBtn;
            var objText = i == 0 ? objTextOne : objTextTwo;

            buffText.text = skill.Description[skill.level];
            buffCD.text = skill.skil.stepMax.ToString();
            buffDMG.text = skill.skil.damage.ToString();
            for (int y = 0; y < buffElementImg.Count; y++)
            {
                buffElementImg[y].sprite = GameManager.Instance.ElementsImg[(int)SkillElements.Find(s => s.skillName == skill.Name).SkillElements[y]];
            }
            buffBtn.sprite = GameManager.ExtractSpriteListFromTexture("skills").First(s => s.name == skill.Name);
            objText.tagText = skill.tag[skill.level];
            list.Add(buffText.gameObject);
        }
        GameManager.Instance.UpdateText(list);
    }
    public List<int> GetTwoRandomNumbers(List<SavedSkillsData> list)
    {
        int index1 = list[Random.Range(0, list.Count)].ID;
        int index2 = list[Random.Range(0, list.Count)].ID;

        while (index1 == index2)
        {
            index2 = list[Random.Range(0, list.Count)].ID;
        }

        return new List<int> { index1, index2 };
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
                    foreach (var child in abil.GetComponentsInChildren<CDSkills>())
                    {
                        if (child.abilityId == skillPoint)
                        {
                            child.stats[child.currentStatLevel++].isTrigger = true;
                        }
                    }
                }
            }
            if (resultId.level == 0)
            {
                SetActiveAbil(player, abil, skillPoint,false);

                isSkillIcons[skillPoint] = true;
                player.abilitiesObj[player.countActiveAbilities - 1].GetComponent<CDSkills>().number = player.countActiveAbilities - 1;

                if (resultId.isPassive)
                {
                    player.abilitiesObj[player.countActiveAbilities - 1].GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
                }
            }
            ModifyJsonField(resultId, ++resultId.level);
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

        SkillIconsMax = skillsSave.Count;
        gameManager.ClosePanel(gameManager.levelPanel);
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
    public void SetSkillData(CDSkills skill, int skillPoint)
    {
        skill.skillMono = resultId.skil.objToSpawn;
        skill.skill = resultId.skil;
        skill.stats = resultId.skil.stats;
        skill.stats[skill.currentStatLevel++].isTrigger = true;
        skill.abilityId = skillPoint;
        skill.type = resultId.type;
    }
    //Activate skill if it`s not active
    public void SetActiveAbil(PlayerManager player, GameObject abilObj, int skillPoint, bool isFirst)
    {
        if (isFirst)
        {
            resultId = skillsSave[skillPoint];
        }
        else
        {
            player.abilitiesObj[player.countActiveAbilities].SetActive(true);
            player.abilities[player.countActiveAbilities].sprite = GameManager.ExtractSpriteListFromTexture("skills").First(s => s.name == resultId.Name);
            player.countActiveAbilities++;
        }

        CDSkills skill = abilObj.transform.GetChild(player.countActiveAbilities - 1).GetComponent<CDSkills>();
        SetSkillData(skill, skillPoint);
        if (isFirst)
        {
            gameManager.ClosePanel(gameManager.levelPanel);
        }
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

                //data.Image = Resources.Load<Sprite>(data.Name);

                itemsRead.Add(data);
            }
        }
        else
        {
            File.Create(path);
        }
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
                data.tag = item.tag;
                data.tagRare = item.tagRare;
                data.isPassive = item.isPassive;
                data.skil = item.skil;
                data.type = item.type;

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

