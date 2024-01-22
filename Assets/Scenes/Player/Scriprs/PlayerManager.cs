using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class PlayerManager : MonoBehaviour
{
    GameManager gameManager;
    Animator playerAnim;
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
    public int secondBulletCount;
    [HideInInspector]
    public bool isRicoshet;
    [HideInInspector]
    public bool isLifeSteal;
    [HideInInspector]
    public bool isBulletSlow;

    [Header("Expiriance settings")]
    public Image expiriencepoint;
    public float level;
    public float expNeedToNewLevel;
    public GameObject levelUp;
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
    private void Awake()
    {
        instance ??= this;
        //Time.timeScale = 1f;
    }
    private void OnDestroy()
    {
        instance = null;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameManager.Instance;
        attackSpeedMax = attackSpeed;
        playerHealthPointMax = playerHealthPoint;
        speedMax = speed;
        countActiveAbilities = 1;
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
    private void Update()
    {
        BaseSkillSelector();
    }
    //Health
    public void HitEnd()
    {
        playerAnim.SetBool("IsHit", false);
        if (playerHealthPoint <= 0)
        {
            AudioManager.instance.MusicStop();
            AudioManager.instance.PlaySFX("PlayerDeath");
            gameManager.OpenPanel(gameManager.losePanel);
        }
    }
    public void TakeDamage(float damage)
    {
        if (damage > 0)
        {
            //Armor
            damage = Armor(damage, armor);
            //Damage deal
            playerHealthPoint -= damage;
            gameManager.FindStatName("DamageTaken", damage);
            fullFillImage.fillAmount -= damage / playerHealthPointMax;
        }
        playerAnim.SetBool("IsHit", true);
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
                StartCoroutine(SlowPlayer(0.2f, 0.7f));
                axisX = horizontalInput;
            }
        }
        else if (verticalInput != 0)
        {
            playerAnim.SetBool("IsMove", true);
            if (axisY != verticalInput)
            {
                StartCoroutine(SlowPlayer(0.2f, 0.7f));
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
        if (speed > speedMax * percent)
        {
            speed = speedMax * percent;
            yield return new WaitForSeconds(time);
            speed = speedMax;
        }
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
            Invoke(nameof(StopUntouchible), 4f);
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
            Vector2 dashDirection = GetMousDirection(transform.position);
            rb.velocity = dashDirection.normalized * (speed * dashMultiplier);
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
        Vector3 direction = GetMousDirection(transform.position);
        direction = direction.normalized * circleRadius;

        // Визначаємо кінцеву позицію об'єкту на колі
        Vector3 targetPosition = transform.position + direction;

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
        foreach (var character in characters)
        {
            foreach (var info in gameManager.charactersRead)
            {
                if (character.id == info.ID && info.isEquiped)
                {
                    playerHealthPointMax = character.health;
                    playerHealthPoint = character.health;

                    speedMax = character.moveSpeed;
                    speed = character.moveSpeed;
                    heroID = character.id;
                    baseSkillCDMax = character.spellCD;

                    attackSpeedMax = character.attackSpeed;
                    attackSpeed = character.attackSpeed;
                    damageToGive = character.damage;
                    break;
                }
            }
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