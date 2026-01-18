using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SkillBaseMono;
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

    [SerializeField]
    public List<status> status;

    public SpellType type;
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

    private Coroutine activeSpawn;

    public KeyCode keyCode;
    TextMeshProUGUI cDText;
    // Update is called once per frame
    void Start()
    {
        skill.skill = this;
        skillMono.Set(skill,number, status);
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
    public IEnumerator SpawnBullets(SkillBaseMono skillMono, SpellType type, int count, float delay, float damageMultiplier, int baseOrder, float spreadAngle = 0f)
    {
        for (int i = 0; i < count; i++)
        {
            SkillBaseMono spell = skillMono.CreateSpellByType(type, skillMono, skillMono, skillMono.currentLevel, damageMultiplier);

            if (spell.skillId == 0)
            {
                spell.GetComponent<Bullet>().spawnPoint = PlayerManager.instance.ShootPoint.transform;

                // одразу налаштовуємо кулям порядок та offset
                var renderers = spell.GetComponentsInChildren<SpriteRenderer>();
                foreach (var r in renderers)
                    r.sortingOrder = baseOrder + i; // кожна наступна куля зверху

                var rb = spell.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float angle = (count == 1) ? 0f : (i - (count - 1) / 2f) * spreadAngle;
                    spell.transform.rotation *= Quaternion.Euler(0, 0, angle);
                    rb.velocity = spell.transform.right * 10f;
                }
            }
            if (i < count - 1) // між кулями робимо паузу
                yield return new WaitForSeconds(delay);
        }
    }
    void Spawn(int counter)
    {
        if (activeSpawn != null) // вже йде серія куль → не запускаємо нову
            return;

        activeSpawn = StartCoroutine(SpawnBulletsRoutine(counter));
    }
    private IEnumerator SpawnBulletsRoutine(int counter)
    {
        yield return StartCoroutine(
            SpawnBullets(skillMono, type, counter, skill.spawnDelay, skillMono.damageMultiplier, 10, 12f)
        );
        AudioManager.instance.PlaySFX(skill.spawnSFXName);

        activeSpawn = null; // дозволяємо наступний спавн
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
