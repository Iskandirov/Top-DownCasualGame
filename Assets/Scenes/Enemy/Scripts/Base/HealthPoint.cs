using UnityEngine;
public class HealthPoint : MonoBehaviour
{
    //public float healthPoint;
    //public float healthPointMax;
    //public bool IsBobs;
    //public Animator anim;
    //public GameObject bodyAnim;
    //public GameObject VFX_Deadarticle;

    //[Header("Burn info")]
    //public bool isBurn;
    //public float burnTime;
    //public float burnDamage;
    //public float burnTick;
    //public float burnTickMax;


    //public DropItems objDrop;
    //public Forward objMove;

    //[Header("Exp info")]
    //public float expGiven;
    //public float dangerLevel;
    //public EXP expiriancePoint;


    //[Header("Boss Anim")]
    //public Boss_Destroy bodyAnimBoss;
    //public bool AnimBossStart;
    //public bool isBossPart;
    //CutThePart objPart;

    //[Header("Effects resistace")]
    //public float Fire;
    //public float Electricity;
    //public float Water;
    //public float Dirt;
    //public float Wind;
    //public float Grass;
    //public float Steam;
    //public float Cold;

    //public float FireStart;
    //public float ElectricityStart;
    //public float WaterStart;
    //public float DirtStart;
    //public float WindStart;
    //public float GrassStart;
    //public float SteamStart;
    //public float ColdStart;

    //public GameObject debuffsParent;
    //public string nameMob;

    //public GameObject uiParent;
    //public GameObject healthBossObj;
    //GameObject health;
    //PlayerManager player;
    //EnemyController spawner;
    //Transform objTransform;
    // Start is called before the first frame update
    //void Start()
    //{
    //    //if (GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) > 0)
    //    //    healthPointMax += GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) * 1.3f;
    //    //healthPoint = healthPointMax;
    //    //if (bodyAnimBoss != null)
    //    //{
    //    //    objPart = GetComponent<CutThePart>();
    //    //}
    //    //player = PlayerManager.instance;
    //    //spawner = EnemyController.instance;
    //    //objTransform = transform;
    //    //if (IsBobs)
    //    //{
    //    //    uiParent = GameObject.Find("/UI");
    //    //    health = Instantiate(healthBossObj, uiParent.transform);
    //    //    health.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 2 - 100f, Screen.height / 2 - 130f);
    //    //    health.GetComponentInChildren<Image>().fillAmount = 1;
    //    //}
    //}
    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    //if (isBurn)
    //    //{
    //    //    burnTime -= Time.fixedDeltaTime;
    //    //    if (burnTime <= 0)
    //    //    {
    //    //        isBurn = false;
    //    //    }
    //    //    burnTick -= Time.fixedDeltaTime;
    //    //    if (burnTick <= 0)
    //    //    {
    //    //        healthPoint -= burnDamage;
    //    //        burnTick = burnTickMax;
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    //End Fire Anim
    //    //    //ChangeToNotKick();
    //    //}
    //    //if (healthPoint <= 0)
    //    //{
    //        //if (IsBobs)
    //        //{
    //        //    if (isBossPart)
    //        //    {
    //        //        SetOtherPartsCount();
    //        //        //ExpGive();
    //        //        Destroy(gameObject);
    //        //    }
    //        //    else if (bodyAnimBoss != null && !AnimBossStart)
    //        //    {
    //        //        player.expiriencepoint.fillAmount += expGiven * dangerLevel / player.expNeedToNewLevel;
    //        //        GameManager.Instance.score++;
    //        //        foreach (var part in bodyAnimBoss.parts)
    //        //        {
    //        //            if (part == gameObject.name)
    //        //            {
    //        //                bodyAnimBoss.GetParts(objPart, healthPointMax);
    //        //                bodyAnimBoss.DestroyStart();
    //        //            }
    //        //        }
    //        //        AnimBossStart = true;
    //        //        bodyAnimBoss = null;
    //        //    }
    //        //    else if (!AnimBossStart)
    //        //    {
    //        //        objDrop.OnDestroyBoss(health);
    //        //        //ExpGive();
    //        //        Destroy(objTransform.root.gameObject);
    //        //    }
    //        //}
    //        //else
    //        //{
    //            //ExpGive();
    //            //healthPoint = healthPointMax;
    //            //if (objMove != null)
    //            //{ 
    //            //    // Генеруємо випадкові координати в межах заданої області спавну
    //            //    Vector3 spawnPosition = spawner.GetRandomSpawnPosition();
    //            //    objMove.transform.position = spawnPosition;
    //            //}
    //            //else
    //            //{
    //            //    spawner.SpawnObjectInMapBounds(objTransform.parent.gameObject);
    //            //}
    //        //}
    //        //if (GetComponentInParent<ElementActiveDebuff>() != null)
    //        //{
    //        //    GetComponentInParent<ElementActiveDebuff>().DeactiveDebuff();
    //        //    foreach (Transform item in debuffsParent.transform)
    //        //    {
    //        //        item.GetComponentInChildren<DeactivateDebuff>().Destroy();
    //        //    }
    //        //}
    //        //GameManager.Instance.FindStatName(nameMob, 1);
    //    //}
    //}
    
    //void ExpGive()
    //{
    //    EXP a = Instantiate(expiriancePoint, new Vector3(objTransform.position.x, objTransform.position.y, 1.9f), Quaternion.identity);
    //    a.expBuff = expGiven * dangerLevel;

    //    player.expiriencepoint.fillAmount += expGiven * dangerLevel / player.expNeedToNewLevel;
    //    GameManager.Instance.score++;
    //}
    //public void SetOthwerPartsCount()
    //{
    //    // Знаходимо всі об'єкти в сцені з компонентом MyComponent
    //    HealthPoint[] objects = FindObjectsOfType<HealthPoint>();

    //    // Проходимося по знайдених об'єктах
    //    foreach (HealthPoint obj in objects)
    //    {
    //        CutThePart part = obj.GetComponent<CutThePart>();
    //        // Перевіряємо значення булевої змінної у кожному об'єкті
    //        if (obj.isBossPart)
    //        {
    //            if (part.countParts == 1)
    //            {
    //                objDrop.OnDestroyBoss(health);
    //            }
    //            part.countParts--;
    //        }
    //    }
    //}
    //public void TakeDamage(float damage)
    //{
    //    healthPoint -= damage;
    //    if (IsBobs)
    //    {
    //        health.GetComponentInChildren<Image>().fillAmount -= damage / healthPointMax;
    //    }
    //}
}
