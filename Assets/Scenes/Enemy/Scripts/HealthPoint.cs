using UnityEngine;

public class HealthPoint : MonoBehaviour
{
    public GameObject player;
    public float stepKick;
    public float stepKickMax;
    public bool isKick;
    public float healthPoint;
    public float healthPointMax;
    public EXP expiriancePoint;
    public bool IsBobs;
    public Animator anim;
    public GameObject bodyAnim;
    public GameObject VFX_Deadarticle;

    [Header("Burn info")]
    public bool isBurn;
    public float burnTime;
    public float burnDamage;
    public float burnTick;
    public float burnTickMax;


    SpriteRenderer[] objsSprite;
    Expirience objExp;
    DropItems objDrop;
    Forward objMove;
    KillCount objKills;

    [Header("Exp info")]
    public float expGiven;
    public float dangerLevel;

    private Camera mainCamera;
    float spawnRadius = 80f;

    [Header("Boss Anim")]
    public Boss_Destroy bodyAnimBoss;
    public bool AnimBossStart;
    public bool isBossPart;
    CutThePart objPart;

    [Header("Effects resistace")]
    public float Fire;
    public float Electricity;
    public float Water;
    public float Dirt;
    public float Wind;
    public float Grass;
    public float Steam;
    public float Cold;

    public float FireStart;
    public float ElectricityStart;
    public float WaterStart;
    public float DirtStart;
    public float WindStart;
    public float GrassStart;
    public float SteamStart;
    public float ColdStart;

    public GameObject debuffsParent;
    Timer defeatBoss;
    // Start is called before the first frame update
    void Start()
    {
        healthPoint = healthPointMax;
        if (bodyAnimBoss != null)
        {
            objPart = GetComponent<CutThePart>();
        }
        player = GameObject.FindWithTag("Player");
        objsSprite = GetComponentsInChildren<SpriteRenderer>();
        objExp = player.GetComponent<Expirience>();
        objDrop = GetComponentInParent<DropItems>();
        objMove = GetComponentInParent<Forward>();
        objKills = FindObjectOfType<KillCount>();
        defeatBoss = FindObjectOfType<Timer>();
    }
   

    private bool IsInsideCameraBounds(Vector3 position)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
        return viewportPosition.x >= -0f && viewportPosition.x <= 1f && viewportPosition.y >= -0f && viewportPosition.y <= 1f;
    }

    private bool IsInsideWallBounds(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f); // Використовуємо OverlapCircleAll замість OverlapPointAll
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition;
        do
        {
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * spawnRadius;
            spawnPosition = new Vector3(mainCamera.transform.position.x + spawnOffset.x, mainCamera.transform.position.y + spawnOffset.y, 1.8f);
        } while (IsInsideCameraBounds(spawnPosition) || IsInsideWallBounds(spawnPosition));

        return spawnPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBurn)
        {
            burnTime -= Time.deltaTime;
            if (burnTime <= 0)
            {
                isBurn = false;
            }
            burnTick -= Time.deltaTime;
            if (burnTick <= 0)
            {
                ChangeToKick();
                healthPoint -= burnDamage;
                ChangeToNotKick();
                burnTick = burnTickMax;
            }
        }
        else
        {
            ChangeToNotKick();
        }
        if (healthPoint <= 0)
        {
            if (IsBobs)
            {
                if (isBossPart)
                {
                    SetOtherPartsCount();
                    EXP a = Instantiate(expiriancePoint, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1.9f), Quaternion.identity);
                    a.expBuff = expGiven * dangerLevel;
                    objExp.expiriencepoint.fillAmount += expGiven * dangerLevel / objExp.expNeedToNewLevel;
                    objKills.score += 1;
                    Destroy(gameObject);
                }
                else if (bodyAnimBoss != null && !AnimBossStart)
                {
                    objExp.expiriencepoint.fillAmount += expGiven * dangerLevel / objExp.expNeedToNewLevel;
                    objKills.score += 1;
                    foreach (var part in bodyAnimBoss.parts)
                    {
                        if (part == gameObject.name)
                        {
                            bodyAnimBoss.GetParts(objPart, healthPointMax);
                            bodyAnimBoss.DestroyStart();
                        }
                    }
                    AnimBossStart = true;
                    bodyAnimBoss = null;
                }
                else if (!AnimBossStart)
                {
                    objDrop.OnDestroyBoss();
                    EXP a = Instantiate(expiriancePoint, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1.9f), Quaternion.identity);
                    a.expBuff = expGiven * dangerLevel;
                    objExp.expiriencepoint.fillAmount += expGiven * dangerLevel / objExp.expNeedToNewLevel;
                    objKills.score += 1;
                    defeatBoss.isBossDefeated = true;
                    Destroy(transform.root.gameObject);
                }
            }
            else
            {
                EXP a = Instantiate(expiriancePoint, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1.9f), Quaternion.identity);
                a.expBuff = expGiven * dangerLevel;

                objExp.expiriencepoint.fillAmount += expGiven * dangerLevel / objExp.expNeedToNewLevel;
                objKills.score += 1;
                mainCamera = Camera.main;
                if (objMove != null)
                {
                    // Генеруємо випадкові координати в межах заданої області спавну
                    Vector3 spawnPosition = GetRandomSpawnPosition();
                    objMove.transform.position = spawnPosition;
                    healthPoint = healthPointMax;

                }
                else
                {
                    
                    Destroy(transform.parent.gameObject);
                }
            }
            if (GetComponentInParent<ElementActiveDebuff>() != null)
            {
                GetComponentInParent<ElementActiveDebuff>().DeactiveDebuff();
                foreach (Transform item in debuffsParent.transform)
                {
                    item.GetComponentInChildren<DeactivateDebuff>().Destroy();
                }
            }
        }
    }
    public void SetOtherPartsCount()
    {
        // Знаходимо всі об'єкти в сцені з компонентом MyComponent
        HealthPoint[] objects = FindObjectsOfType<HealthPoint>();

        // Проходимося по знайдених об'єктах
        foreach (HealthPoint obj in objects)
        {
            // Перевіряємо значення булевої змінної у кожному об'єкті
            if (obj.isBossPart)
            {
                if (obj.GetComponent<CutThePart>().countParts == 1)
                {
                    defeatBoss.isBossDefeated = true;
                    objDrop.OnDestroyBoss();
                }
                obj.GetComponent<CutThePart>().countParts--;
            }
        }
    }
    public void ChangeToKick()
    {
        for (int i = 0; i < objsSprite.Length; i++)
        {
            objsSprite[i].color = new Color32(255, 0, 0, 255);
        }
        isKick = true;
    }
    public void ChangeToNotKick()
    {
        if (isKick == true)
        {
            stepKick -= Time.deltaTime;
            if (stepKick <= 0)
            {
                for (int i = 0; i < objsSprite.Length; i++)
                {
                    objsSprite[i].color = new Color32(255, 255, 255, 255);
                }
                isKick = false;
                stepKick = stepKickMax;
            }
        }
    }
}
