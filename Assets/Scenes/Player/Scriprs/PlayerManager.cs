using Cinemachine;
using FSMC.Runtime;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float callDown;
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
    PotionShield,    
    EquipmentBuff,
}
public enum PotionsType
{
    Heal,
    Bomb,    
    Totem,
    TimeFreeze,
    None,

}
public class PlayerManager : MonoBehaviour
{
    public GameManager gameManager;
    public Animator playerAnim;
    public Rigidbody2D rb;
    [HideInInspector]
    public static PlayerManager instance;
    public List<GameObject> characterPrefabs;

    [Header("Move settings")]
    public float speed;
    [HideInInspector]
    public float speedMax;
    [HideInInspector]
    public float baseSkillCD;
    public float baseSkillCDMax;
    
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
    public SkeletonAnimation anim;

    [Header("Health settings")]
    public float playerHealthPoint;
    [HideInInspector]
    public float playerHealthPointMax;
    public float playerHealthRegeneration;
    public float armor;
    public bool isInvincible = false;
    public TextMeshProUGUI regenerationText;

    [Header("Shoot settings")]
    public Bullet bullet;
    public List<Bullet> bulletsPool;
    public GameObject ShootPoint;
    public float lookAngle;
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
    public Sprite[] autoState;
    public float offset;

    [Header("Expiriance settings")]
    public float level;
    public float expNeedToNewLevel;
    public GameObject levelUpgradeEffect;
    //public GameObject levelUp;
    [HideInInspector]
    public int isEnemyInZone;
    public float multiply;

    [Header("Elemental settings")]
    public float Fire;
    public float Electricity;
    public float Water;
    public float Dirt;
    public float Wind;
    public float Grass;
    public float Steam;
    public float Cold;

    [Header("Abilities Buff")]
    public GameObject buffObj;
    public GameObject debuffObj;
    public LoadEquipedItems itemsFromBoss;
    [Header("CharacterSet settings")]
    public List<CharacterStats> characters;
    [HideInInspector]
    public Transform objTransform;
    [Header("Perks settings")]
    [SerializeField] List<PerksBuffs> statsToBuff;
    [SerializeField] Shield PotionShield;
    public int activePerkCount;
    [Header("Potions settings")]
    [SerializeField] List<Potions> potions;
    [SerializeField] BobmExplode bomb;
    [SerializeField] Sprite bombImg;
    [SerializeField] bool canBeSaved = false;
    [SerializeField] GameObject potionUseVFX;
    public int countActivePotions = 0;
    private float targetRotation = 0f;
    [SerializeField] GameObject baseSkillVFX;
    [Header("Particle settings")]
    ParticleSystem particleVibe;
    Vector2 particleVelocityDirection;
    Vector2 targetVelocity; // рух гравця

    float smoothTime = 0.5f; // час затримки зміни напрямку
    float velocitySmoothing; // службова змінна для згладжування

    private void Awake()
    {
        instance ??= this;
        objTransform = transform;
        if (GameObject.Find("WorldUI/Vibe") != null)
        {
            particleVibe = GameObject.Find("WorldUI/Vibe").GetComponent<ParticleSystem>();

        }

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        originalPos = transform.localPosition;
        if (mainCamera == null) mainCamera = Camera.main;
    }
    private void OnDestroy()
    {
        instance = null;
        OnAttackTypeSwitch -= OnSwitchClickHandler;
        foreach (var potion in potions)
        {
            if (PlayerPrefs.GetString(potion.key + "Bool") == "True")
            {
                PlayerPrefs.SetString(potion.key + "Bool",false.ToString());
            }
        }
        LoadPotions.SetPotionValue(potions, true);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        regenerationText = GameObject.FindGameObjectWithTag("Health").GetComponent<TextMeshProUGUI>();
        foreach (var perk in statsToBuff)
        {
            if (PlayerPrefs.HasKey(perk.key))
            {
                perk.value = PlayerPrefs.GetFloat(perk.key);
            }
        }
        
        foreach (var potion in potions)
        {
            if (PlayerPrefs.GetString(potion.key + "Bool") == "True")
            {
                potion.isActive = bool.Parse(PlayerPrefs.GetString(potion.key + "Bool"));
                if (gameManager.potionsObj.Count > 0)
                {
                    Image img = gameManager.potionsObj[countActivePotions].GetComponent<Image>();
                    img.sprite = GameManager.ExtractSpriteListFromTexture("Quest").First(instance => instance.name == potion.key);
                    img.SetNativeSize();
                    img.color = new Color(255, 255, 255, 255);
                    gameManager.potionsObj[countActivePotions].type = potion.parameters;
                    gameManager.potionsObj[countActivePotions].callDownMax = potion.callDown;
                    countActivePotions++;
                }
               
            }
        }
        attackSpeedMax = attackSpeed;
        playerHealthPointMax = playerHealthPoint;
        SetCharacterOnStart();
        OnAttackTypeSwitch += OnSwitchClickHandler;
    }
    void MoveParticles(Vector2 direction)
    {
        ParticleSystem.Particle[] particlesArray = new ParticleSystem.Particle[particleVibe.main.maxParticles];
        int count = particleVibe.GetParticles(particlesArray);

        Vector3 moveDirection = new Vector3(direction.x, direction.y, 0).normalized;
        float baseParticleSpeed = particleVibe.main.startSpeedMultiplier;
        for (int i = 0; i < count; i++)
        {
            float speedMultiplier = 1f / particlesArray[i].GetCurrentSize(particleVibe);
            particlesArray[i].position += moveDirection * baseParticleSpeed * speedMultiplier * Time.deltaTime;
        }

        particleVibe.SetParticles(particlesArray, count);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //lookAngle = Mathf.Atan2(GetMousDirection(objTransform.position).y, GetMousDirection(objTransform.position).x) * Mathf.Rad2Deg;
        //if (Input.GetMouseButton(0))
        //{
        //    GameObject bulletClone = Instantiate(bullet.gameObject);
        //    bulletClone.transform.position = ShootPoint.transform.position;
        //    bulletClone.transform.rotation = Quaternion.Euler(0, 0, lookAngle);

        //    bulletClone.GetComponent<Rigidbody2D>().velocity = ShootPoint.transform.right * bulletClone.GetComponent<Bullet>().launchForce;
        //    Debug.Log("Shoot");
        //}
        if (particleVibe != null)
        {
            targetVelocity = -rb.velocity;
            // плавно змінюємо напрямок частинок
            particleVelocityDirection = Vector2.Lerp(
                particleVelocityDirection,
                targetVelocity,
                Time.deltaTime / smoothTime
            );
            // рухаємо частинки у напрямку particleVelocityDirection
            MoveParticles(particleVelocityDirection);
        }
        //ArrowToCursor();
        CDBaseSkill();
        if (!isDashSkillActive)
        {
            rb.position = PlayerMove();
        }
        regenerationText.text = playerHealthRegeneration.ToString("F1");
        //ShootBullet(gameObject);
        if (playerHealthRegeneration > 0 && playerHealthPoint < playerHealthPointMax)
        {
            playerHealthPoint += playerHealthRegeneration / playerHealthPointMax;
            gameManager.fullFillImage.fillAmount = playerHealthPoint / playerHealthPointMax;
        }
    }
    //Тестовий делегат
    public delegate void AutoAttackSwitchHandler();
    public event AutoAttackSwitchHandler OnAttackTypeSwitch;
    private void OnSwitchClickHandler()
    {
        gameManager.autoimage.sprite = isAuto ? autoState[0] : autoState[1];
        gameManager.autoimage.SetNativeSize();
        isAuto = !isAuto;
        gameManager.AutoActiveCurve.gameObject.SetActive(isAuto);
       
    }

    //Кінець тестовому делегату
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !LootManager.inst.isTutor)
        {
            OnAttackTypeSwitch?.Invoke();
        }
        if (isAuto && gameManager.AutoActiveCurve != null)
        {
            // Отримати поточне значення офсету
            offset = gameManager.AutoActiveCurve.GetFloat("offset");

            // Додати до офсету значення Time.deltaTime, щоб змінювати його на 1 за секунду
            offset += Time.deltaTime;

            // Встановити нове значення офсету
            gameManager.AutoActiveCurve.SetFloat("offset", offset);
        }
        BaseSkillSelector();
    }
    //Health
    [Header("Hit settings")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = new Color(1f, 0.4f, 0.4f); // червоний для dmg
    public float flashDuration = 0.12f;
    public ParticleSystem hitParticles;
    public float knockbackAmount = 0.05f;
    public float knockbackDuration = 0.08f;
    public Camera mainCamera;
    public float cameraShakeIntensity = 0.03f;
    public float cameraShakeDuration = 0.08f;

    Vector3 originalPos;

    public void OnHit(int damage)
    {
        //StopAllCoroutines();
        StartCoroutine(HitRoutine(damage));
    }

    IEnumerator HitRoutine(int damage)
    {
        originalPos = transform.localPosition;
        // 1) Flash sprite
        Color original = spriteRenderer.color;
        spriteRenderer.color = hitColor;
        if (hitParticles) hitParticles.Play();

        // 2) Small knockback (to the left as example). Adjust direction by attacker position if you have it.
        Vector3 target = originalPos + Vector3.left * knockbackAmount;
        float t = 0f;
        while (t < knockbackDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPos, target, t / knockbackDuration);
            t += Time.deltaTime;
            yield return null;
        }

        // 3) restore position and sprite color
        transform.localPosition = originalPos;
        spriteRenderer.color = original;

        // 4) tiny camera shake
        mainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CineMachineCameraShake>().Shake(10,.2f);
    }

    public void HitEnd()
    {
        OnHit(0);
        playerAnim.SetTrigger("Hit");
        if (playerHealthPoint <= 0)
        {
            if (canBeSaved)
            {
                float effectivnessPerk = GivePerkStatValue(Stats.Effectivness);
                float healPercent = 0.2f;
                if (effectivnessPerk > 1)
                {
                    healPercent += 0.2f * 0.4f; // 20% + 40% від 20% = 28%
                }
                playerHealthPoint = playerHealthPointMax * healPercent;

                gameManager.fullFillImage.fillAmount = playerHealthPoint / playerHealthPointMax;
                GameManager.Instance.FindStatName("healthHealed", playerHealthPointMax * 0.2f);
                canBeSaved = false;
            }
            else
            {
                //DailyQuests.instance.UpdateValue(4, 0);
                AudioManager.instance.MusicStop();
                AudioManager.instance.PlaySFX("PlayerDeath");
                gameManager.OpenPanel(gameManager.losePanel, true,true);
                gameManager.TimeScale(0);
            }
        }
    }
    public void TakeDamage(float damage)
    {
        playerAnim.SetTrigger("Hit");

        if (damage > 0 && !isInvincible)
        {
            if (!isTutor)
            {
                //Armor
                damage = Armor(damage, armor);
                //Damage deal
                playerHealthPoint -= damage;
                gameManager.FindStatName("DamageTaken", damage);
                gameManager.fullFillImage.fillAmount -= damage / playerHealthPointMax;
                if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 4 && s.isActive == true) != null)
                {
                    DailyQuests.instance.UpdateValue(4, damage, false, true);
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isReloading)
        {
            baseSkillVFX.SetActive(true);
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
        
       
    }
    public Vector2 PlayerMove()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        anim.AnimationName = horizontalInput == 0 && verticalInput == 0 ? "Idle" : "Run";
        if (horizontalInput != 0)
        {
            playerAnim.SetBool("IsMove", true);
            if (axisX != horizontalInput)
            {
                //StartCoroutine(SlowPlayer(0.2f, 0.9f));
                axisX = horizontalInput;
            }
             targetRotation = horizontalInput > 0 ? 180f : 0f;

            // Плавне обертання до цільового кута
            transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(transform.rotation.eulerAngles.y, targetRotation, 5), 0);
        }
        else if (verticalInput != 0)
        {
            //playerAnim.SetBool("IsMove", true);
            if (axisY != verticalInput)
            {
                //StartCoroutine(SlowPlayer(0.2f, 0.9f));
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
    public void StartSlowPlayer(float time, float percent)
    {
        StartCoroutine(SlowPlayer(time, percent));
    }
    public IEnumerator SlowPlayer(float time, float percent)
    {
        slowArray.Add(percent);
        CalculateSlow(speedMax);
        yield return new WaitForSeconds(time);
        slowArray.Remove(slowArray.Where(slow => slow == percent).First());
        CalculateSlow(speedMax);
    }
    void CalculateSlow(float currentSpeed)
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
        Debug.Log("Ricoshet");
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
        Debug.Log("Untouchible");
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
        Debug.Log("Dash");
        //Dash
        if (!isDashSkillActive)
        {
            isDashSkillActive = true;
            isInvincible = true;
            Vector2 dashDirection = GetMousDirection(objTransform.position);
            rb.velocity = dashDirection.normalized * dashMultiplier;
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            baseSkillVFX.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            Invoke(nameof(StopDashing), .2f);
        }
    }
    //===============STOP SPECIAL SPELL============
    private void StopDashing()
    {
        isDashSkillActive = false;
        isInvincible = false;
        rb.velocity = Vector2.zero;
        Invoke(nameof(StopWithdDelay), 0.8f);
    }
    void StopWithdDelay()
    {
        baseSkillVFX.SetActive(false);
    }
    private void StopUntouchible()
    {
        isBaseSkillActive = false;
        isInvincible = false;
        baseSkillVFX.SetActive(false);
    }
    private void StopRicoshet()
    {
        isBaseSkillActive = false;
        isRicoshet = false;
        baseSkillVFX.SetActive(false);
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
    //public void ArrowToCursor()
    //{
    //    // Визначаємо відстань від гравця до позиції курсора
    //    Vector3 direction = GetMousDirection(objTransform.position);
    //    direction = direction.normalized * circleRadius;

    //    // Визначаємо кінцеву позицію об'єкту на колі
    //    Vector3 targetPosition = objTransform.position + direction;

    //    // Рухаємо об'єкт до кінцевої позиції
    //    ShootPoint.transform.position = targetPosition;

    //    // Повертаємо коло в напрямку курсора
    //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //    ShootPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    //}

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
        CharacterStats character = characters.Find(i => i.id == PlayerPrefs.GetInt("Character") && gameManager.charactersRead.Find(c => c.isEquiped).isEquiped);
        playerHealthPointMax = character.health + GivePerkStatValue(Stats.Health);
        playerHealthPoint = playerHealthPointMax;

        speedMax = character.moveSpeed + GivePerkStatValue(Stats.MoveSpeed);
        speed = speedMax;
        heroID = PlayerPrefs.GetInt("Character");
        baseSkillCDMax = character.spellCD - GivePerkStatValue(Stats.ReloadSkills) / 100;
        attackSpeedMax = character.attackSpeed - GivePerkStatValue(Stats.AttackSpeed) / 100;
        attackSpeed = attackSpeedMax;
        damageToGive = character.damage + GivePerkStatValue(Stats.Damage);
        GetComponent<CircleCollider2D>().radius += GivePerkStatValue(Stats.ExpirianceRadius) / 100;
        Fire += GivePerkStatValue(Stats.FireDamage) / 100;
        Water += GivePerkStatValue(Stats.WaterDamage) / 100;
        armor += GivePerkStatValue(Stats.Armor);
        /*Швидкість захвату зони*/
        playerHealthRegeneration += GivePerkStatValue(Stats.Regeneration);
        multiply += GivePerkStatValue(Stats.ExpirianceGain) / 100;

        //gameObject = characters[character.id + 1];
    }
    public float GivePerkStatValue(Stats stat)
    {
        if (statsToBuff.Find(b => b.parameters.Equals(stat)).value != 0)
        {
            activePerkCount++;
            return statsToBuff.Find(b => b.parameters.Equals(stat)).value;
        }
        else
        {
            return 0;
        }
    }

    //Potion
    public void HealHealth(float value)
    {
        if (playerHealthPoint != playerHealthPointMax)
        {
            if (playerHealthPoint + value <= playerHealthPointMax)
            {
                playerHealthPoint += value;
                gameManager.fullFillImage.fillAmount += (value) / playerHealthPointMax;
                DailyQuests.instance.UpdateValue(1, value, false, true);
                GameManager.Instance.FindStatName("healthHealed", value);
            }
            else
            {
                GameManager.Instance.FindStatName("healthHealed", playerHealthPointMax - playerHealthPoint);
                DailyQuests.instance.UpdateValue(1, playerHealthPointMax - playerHealthPoint, false, true);

                playerHealthPoint = playerHealthPointMax;
                gameManager.fullFillImage.fillAmount = 1f;
            }
        }
    }

    [Obsolete]
    public void UsePotion(PotionsType type)
    {
        switch (type)
        {
            case PotionsType.Heal:
                HealHealth(25 * Grass * (GivePerkStatValue(Stats.Effectivness) + 1));
                StartCoroutine(VFXPotionDestroy(potionUseVFX, Color.green));
                if (GivePerkStatValue(Stats.PotionShield) != 0)
                {
                    Shield shield = Instantiate(PotionShield);
                    shield.isPotions = true;
                    shield.healthShield = playerHealthPointMax * (GivePerkStatValue(Stats.PotionShield));
                }
                
                break;
            case PotionsType.Bomb:
                ThowBomb();
                StartCoroutine(VFXPotionDestroy(potionUseVFX, Color.black));
                break;
            case PotionsType.TimeFreeze:
                TimeFreeze();
                StartCoroutine(VFXPotionDestroy(potionUseVFX,Color.blue));
                break;
            case PotionsType.Totem:
                SaveTotem();
                StartCoroutine(VFXPotionDestroy(potionUseVFX, Color.yellow));
                break; 
            case PotionsType.None:
                Debug.Log("Something went wrong");
                break;
        }
    }
    void ThowBomb()
    {
        BobmExplode a = Instantiate(bomb, objTransform.position, Quaternion.identity);
        a.damage *= ((GivePerkStatValue(Stats.Effectivness) + 1) + GivePerkStatValue(Stats.ExplosionDamage));
    }
    void TimeFreeze()
    {
        EnemySpawner enemies = FindAnyObjectByType<EnemySpawner>();
        foreach (var enemy in enemies.children.FindAll(c => c.gameObject.activeSelf))
        {
            enemy.SetFloat("Stun Time", 2f * (GivePerkStatValue(Stats.Effectivness) / 100 + 1));
            enemy.StateMachine.SetCurrentState("Stun", enemy);
        }
       
    } 
    void SaveTotem()
    {
        canBeSaved = true;
    }

    [Obsolete]
    IEnumerator VFXPotionDestroy(GameObject potionVFX, Color color)
    {
        var renderer = potionVFX.GetComponent<ParticleSystemRenderer>();
        Material mat = renderer.sharedMaterial;

        GameObject a = Instantiate(potionVFX, objTransform.position, Quaternion.identity);
       
        foreach (var particle in a.GetComponentsInChildren<ParticleSystem>())
        {
            particle.startColor = color;
            mat.SetColor("_EmissionColor", color * 2f);
            mat.SetColor("_BaseColor", color * 2f);
        }
        yield return new WaitForSeconds(2f);
        Destroy(a);
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