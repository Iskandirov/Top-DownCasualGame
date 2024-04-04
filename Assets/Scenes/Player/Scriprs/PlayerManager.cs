using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
[Serializable]
class PerksBuffs 
{
    public string key;
    public float value;
    public Stats parameters;
   
}
[Serializable]
public class Potions 
{
    public string key;
    public bool isActive;
    public float value;
    public PotionsType parameters;
}

public enum Stats
{
    Nothing,
    MoveSpeed,
    Damage,
    AttackSpeed,
    Health,
    ExpirianceRadius,
    FireDamage,
    Armor,  
    LoadSpeed,
    WaterDamage,
    Effectivness,
    ReloadSkills,
    ExplosionDamage,
    Regeneration,
    ExpirianceGain,    
    EquipmentBuff,
}
public enum PotionsType
{
    Nothing,
    Heal,
    Bomb,    
    Ligma,
    Totem,
    TimeFreeze,
}

public class PlayerManager : MonoBehaviour
{
    GameManager gameManager;
    public Animator playerAnim;
    public Rigidbody2D rb;
    [HideInInspector]
    public static PlayerManager instance;

    [Header("Move settings")]
    public float speed;
    [HideInInspector]
    public float speedMax;
    [HideInInspector]
    public float baseSkillCD;
    public float baseSkillCDMax;
    public Image spriteCD;
    public TextMeshProUGUI text;
    private bool isReloading;
    [HideInInspector]
    public float axisX;
    [HideInInspector]
    public float axisY;
    public float dashMultiplier = 5;
    [HideInInspector]
    public bool isBaseSkillActive = false;
    [HideInInspector]
    public bool isDashSkillActive = false;
    public bool isTutor;
    public int heroID = 1;
    public List<float> slowArray = new List<float>();

    [Header("Health settings")]
    public float playerHealthPoint;
    [HideInInspector]
    public float playerHealthPointMax;
    public float playerHealthRegeneration;
    public float armor;
    public Image fullFillImage;
    public bool isInvincible = false;
    public bool shildActive;

    [Header("Shoot settings")]
    public Bullet bullet;
    public GameObject ShootPoint;
    public float damageToGive;
    public float attackSpeed;
    [HideInInspector]
    public float attackSpeedMax;
    public float launchForce;  // Сила запуску кулі
    public float lifeStealPercent;
    public float slowPercent;
    [HideInInspector]
    public float circleRadius = 10f;  // Радіус кола
    [HideInInspector]
    public bool isLevelTwo;
    [HideInInspector]
    public bool isLevelFive;
    [HideInInspector]
    public bool isRicoshet;
    [HideInInspector]
    public bool isLifeSteal;
    [HideInInspector]
    public bool isBulletSlow;
    public bool isAuto;
    public Image autoimage;
    public Sprite[] autoState;
    public VisualEffect AutoActiveCurve;
    public float offset;

    [Header("Expiriance settings")]
    public Image expiriencepoint;
    public float level;
    public float expNeedToNewLevel;
    //public GameObject levelUp;
    public Timer time;
    [HideInInspector]
    public int isEnemyInZone;
    public int multiply;

    [Header("Elemental settings")]
    public float Fire;
    public float Electricity;
    public float Water;
    public float Dirt;
    public float Wind;
    public float Grass;
    public float Steam;
    public float Cold;

    [Header("Abilities")]
    public Image[] abilities;
    public GameObject[] abilitiesObj;
    [HideInInspector]
    public int abilityId;
    public int countActiveAbilities;
    [Header("CharacterSet settings")]
    public List<CharacterStats> characters;
    [HideInInspector]
    public Transform objTransform;
    [Header("Perks settings")]
    [SerializeField] List<PerksBuffs> statsToBuff;
    [Header("Potions settings")]
    [SerializeField] List<Potions> potions;
    private void Awake()
    {
        instance ??= this;
        objTransform = transform;
    }
    private void OnDestroy()
    {
        instance = null;
        OnButtonClicked -= OnButtonClickHandler;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var perk in statsToBuff)
        {
            if (PlayerPrefs.HasKey(perk.key))
            {
                perk.value = PlayerPrefs.GetFloat(perk.key);
            }
        } 
        foreach (var potion in potions)
        {
            if (PlayerPrefs.HasKey(potion.key))
            {
                potion.value = PlayerPrefs.GetInt(potion.key);
            }
        }
        gameManager = GameManager.Instance;
        attackSpeedMax = attackSpeed;
        playerHealthPointMax = playerHealthPoint;
        countActiveAbilities = 1;
        SetCharacterOnStart();
        OnButtonClicked += OnButtonClickHandler;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        ArrowToCursor();
        if (!isTutor)
        {
            CDBaseSkill();
        }
       
        if (!isDashSkillActive)
        {
            rb.position = PlayerMove();
        }

        //ShootBullet(gameObject);
        if (playerHealthRegeneration > 0)
        {
            playerHealthPoint += playerHealthRegeneration / playerHealthPointMax;
            fullFillImage.fillAmount += playerHealthRegeneration / playerHealthPointMax;
        }
    }
    //Тестовий делегат
    public delegate void ButtonClickHandler();
    public event ButtonClickHandler OnButtonClicked;
    private void OnButtonClickHandler()
    {
        autoimage.sprite = isAuto ? autoState[0] : autoState[1];
        isAuto = !isAuto;
        AutoActiveCurve.gameObject.SetActive(isAuto);
       
    }

    //Кінець тестовому делегату
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnButtonClicked?.Invoke();
        }
        if (isAuto && AutoActiveCurve != null)
        {
            // Отримати поточне значення офсету
            offset = AutoActiveCurve.GetFloat("offset");

            // Додати до офсету значення Time.deltaTime, щоб змінювати його на 1 за секунду
            offset += Time.deltaTime;

            // Встановити нове значення офсету
            AutoActiveCurve.SetFloat("offset", offset);
        }
        BaseSkillSelector();
    }
    //Health
    public void HitEnd()
    {
        playerAnim.SetBool("IsHit", false);
        if (playerHealthPoint <= 0)
        {
            //DailyQuests.instance.UpdateValue(4, 0);
            AudioManager.instance.MusicStop();
            AudioManager.instance.PlaySFX("PlayerDeath");
            gameManager.OpenPanel(gameManager.losePanel, true);
        }
    }
    public void TakeDamage(float damage)
    {
        playerAnim.SetBool("IsHit", true);

        if (damage > 0 && !isInvincible)
        {
            if (!isTutor)
            {
                //Armor
                damage = Armor(damage, armor);
                //Damage deal
                playerHealthPoint -= damage;
                gameManager.FindStatName("DamageTaken", damage);
                fullFillImage.fillAmount -= damage / playerHealthPointMax;
                if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 4 && s.isActive == true) != null)
                {
                    DailyQuests.instance.UpdateValue(4, damage, false);
                }
            }
        }
    }
    public float Armor(float damage,float armor)
    {
        float absArmor = Mathf.Abs(armor);
        float modifier = (0.052f * armor) / (0.9f + 0.052f * absArmor);
        damage -= damage * modifier;
        return damage;
    }
    //End Health
    //Move
    void BaseSkillSelector()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isReloading && !isTutor)
        {
            isReloading = true;
            //Player special spell
            switch (heroID)
            {
                case 1:
                    Dash();
                    break;
                case 2:
                    Untouchible();
                    break;
                case 3:
                    Ricoshet();
                    break;
            }
            baseSkillCD = baseSkillCDMax;
        }
    }
    public void CDBaseSkill()
    {
        if (isReloading)
        {
            baseSkillCD -= Time.fixedDeltaTime;
            if (baseSkillCD <= 0)
            {
                isReloading = false;
            }
        }
        if (gameManager.IsGamePage)
        {
            text.text = baseSkillCD.ToString("0.");
            spriteCD.fillAmount = baseSkillCD / baseSkillCDMax;
            if (baseSkillCD <= 0)
            {
                text.gameObject.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
            }
        }
       
    }
    public Vector2 PlayerMove()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0)
        {
            playerAnim.SetBool("IsMove", true);
            if (axisX != horizontalInput)
            {
                StartCoroutine(SlowPlayer(0.2f, 0.9f));
                axisX = horizontalInput;
            }
        }
        else if (verticalInput != 0)
        {
            playerAnim.SetBool("IsMove", true);
            if (axisY != verticalInput)
            {
                StartCoroutine(SlowPlayer(0.2f, 0.9f));
                axisY = verticalInput;
            }
        }
        else
        {
            playerAnim.SetBool("IsMove", false);
        }

        rb.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
        return new Vector2(rb.position.x, rb.position.y);
    }
    public IEnumerator SlowPlayer(float time, float percent)
    {
        slowArray.Add(percent);
        CalculateSlow(speedMax);
        yield return new WaitForSeconds(time);
        slowArray.Remove(slowArray.Where(slow => slow == percent).First());
        CalculateSlow(speedMax);
    }
    public void CalculateSlow(float currentSpeed)
    {
        foreach (var slowElement in slowArray)
        {
            currentSpeed *= Math.Max(slowElement, 0.01f);
        }
        currentSpeed = Math.Max(currentSpeed, 5.0f);
        speed = slowArray.Count > 0 ? currentSpeed : speedMax;
    }
    //=================RICOSHET===================
    void Ricoshet()
    {
        if (!isBaseSkillActive)
        {
            isBaseSkillActive = true;
            isRicoshet = true;
            Invoke(nameof(StopRicoshet), 10f);
        }
    }
    //=================RICOSHET END================
    //=================UNTOUCHIBLE=================
    void Untouchible()
    {
        if (!isBaseSkillActive)
        {
            isBaseSkillActive = true;
            isInvincible = true;
            Invoke(nameof(StopUntouchible), 2f);
        }
    }
    //=================UNTOUCHIBLE END=============
    //====================DASH=====================
    void Dash()
    {
        //Dash
        if (!isDashSkillActive)
        {
            isDashSkillActive = true;
            isInvincible = true;
            Vector2 dashDirection = GetMousDirection(objTransform.position);
            rb.velocity = dashDirection.normalized * dashMultiplier;
            Invoke(nameof(StopDashing), .2f);
        }
    }
    //===============STOP SPECIAL SPELL============
    private void StopDashing()
    {
        isDashSkillActive = false;
        isInvincible = false;
        rb.velocity = Vector2.zero;
    }
    private void StopUntouchible()
    {
        isBaseSkillActive = false;
        isInvincible = false;
    }
    private void StopRicoshet()
    {
        isBaseSkillActive = false;
        isRicoshet = false;
    }
    //===============STOP SPECIAL SPELL END========
    public Vector2 GetMousDirection(Vector3 position)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerToMouse = mousePosition - position;
        return playerToMouse.normalized;
    }
    //====================DASH END=================
    //End Move
    //Shoot
    public void ArrowToCursor()
    {
        // Визначаємо відстань від гравця до позиції курсора
        Vector3 direction = GetMousDirection(objTransform.position);
        direction = direction.normalized * circleRadius;

        // Визначаємо кінцеву позицію об'єкту на колі
        Vector3 targetPosition = objTransform.position + direction;

        // Рухаємо об'єкт до кінцевої позиції
        ShootPoint.transform.position = targetPosition;

        // Повертаємо коло в напрямку курсора
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ShootPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public void ShootBullet(Vector3 position, Bullet newObject)
    {
        newObject.transform.position = position;

        Vector2 directionBullet = GetMousDirection(position);
        // Запускаємо об'єкт в заданому напрямку
        Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
        rb.AddForce(directionBullet.normalized * launchForce, ForceMode2D.Impulse);
        float angleShot = Mathf.Atan2(directionBullet.y, directionBullet.x) * Mathf.Rad2Deg;
        newObject.transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
    }
    public void AutoShoot(Vector3 position, Bullet newObject)
    {

        newObject.transform.position = position;

        Vector2 nearest = new Vector3(999, 999, 999);
        float nearestDistSqr = Mathf.Infinity;

        foreach (var enemy in EnemyController.instance.children)
        {
            Vector3 enemyPos = enemy.objTransform.position;
            float distSqr = (enemyPos - position).sqrMagnitude;

            if (distSqr < nearestDistSqr)
            {
                nearestDistSqr = distSqr;
                nearest = enemyPos;
            }
        }
        Vector2 myPos = new Vector2(objTransform.position.x, objTransform.position.y);
        Vector2 direction = nearest - myPos;
        direction.Normalize();

        // Запускаємо об'єкт в заданому напрямку
        Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
        rb.AddForce(direction.normalized * launchForce, ForceMode2D.Impulse);
        float angleShot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        newObject.transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
    }
    
    //End Shoot
    //Expiriance
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            isEnemyInZone++;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            isEnemyInZone--;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Expirience"))
        {
            collision.GetComponent<EXP>().itWasInPlayerZone = true;
        }
    }
    //End Expiriance
    //Character set
    public void SetCharacterOnStart()
    {
        foreach (var info in gameManager.charactersRead)
        {
            CharacterStats character = characters.Find(i => i.id == info.ID && info.isEquiped);

            playerHealthPointMax = character.health + GivePerkStatValue(Stats.Health);
            playerHealthPoint = character.health + GivePerkStatValue(Stats.Health);

            speedMax = character.moveSpeed + GivePerkStatValue(Stats.MoveSpeed);
            speed = character.moveSpeed + GivePerkStatValue(Stats.MoveSpeed);
            heroID = character.id;
            baseSkillCDMax = character.spellCD + GivePerkStatValue(Stats.ReloadSkills);

            attackSpeedMax = character.attackSpeed + GivePerkStatValue(Stats.AttackSpeed);
            attackSpeed = character.attackSpeed + GivePerkStatValue(Stats.AttackSpeed);
            damageToGive = character.damage + GivePerkStatValue(Stats.Damage);
            GetComponent<CircleCollider2D>().radius += GivePerkStatValue(Stats.ExpirianceRadius);
            Fire += GivePerkStatValue(Stats.FireDamage) / 100;
            Water += GivePerkStatValue(Stats.WaterDamage) / 100;
            armor += GivePerkStatValue(Stats.Armor);
            /*Швидкість захвату зони*/
            playerHealthRegeneration += GivePerkStatValue(Stats.Regeneration);
            multiply += (int)GivePerkStatValue(Stats.ExpirianceGain);
            break;
        }
        //foreach (var potion in potions)
        //{
        //    potion.isActive = bool.Parse(PlayerPrefs.GetInt(potion.key+"Bool").ToString());
        //}
    }
    float GivePerkStatValue(Stats stat)
    {
        if (statsToBuff.Find(b => b.parameters.Equals(stat)).value != 0)
        {
            return statsToBuff.Find(b => b.parameters.Equals(stat)).value;
        }
        else
        {
            return 0;
        }
    }
}

[System.Serializable]
public class CharacterStats
{
    public int id;
    public int health;
    public int moveSpeed;
    public int damage;
    public float attackSpeed;
    public float spellCD;
}