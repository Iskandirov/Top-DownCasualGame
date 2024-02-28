using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Enemy;

[Serializable]
public class EnemyPool 
{
    public GameObject enemyObj;
    public List<GameObject> enemyPool;
    public int poolSize;
}
[Serializable]
public class Enemy
{
    [field: SerializeField] public string name { get; private set; }
    [field: SerializeField] public string type { get; private set; }
    [field: SerializeField] public EnemyState prefab { get; private set; }
    private float health { get; set; }
    [field: SerializeField] public float healthMax { get; private set; }
    [field: SerializeField] public float speedMax { get; private set; }
    private float damage { get; set; }
    [field: SerializeField] public float damageMax { get; private set; }
    private float attackSpeed { get; set; }
    [field: SerializeField] public float attackSpeedMax { get; private set; }
    [field: SerializeField] public int dangerLevel { get; private set; }
    
    [field: SerializeField] public int maxCountPerTime { get; private set; }
    [SerializeField] public enum SpawnType { Camera, Map }
    
    [SerializeField] public AIPath path;
    [SerializeField] public float expGiven;
    public Enemy()
    {
        health = healthMax;
        damage = damageMax;
        attackSpeed = attackSpeedMax;
        maxCountPerTime = 1;
    }
    public interface IEnemy
    {
        GameObject Attack(EnemyState enemy);
        void Death(EnemyState enemy);
    }
    
    public void HealthChange(float value)
    {
        healthMax = value;
    }
    public void SetSpeed(float value)
    {
        speedMax = value;
    }
    public void DamageReduce(float value)
    {
        damageMax = value;
    }
    public void AttackSpeedReduce(float value)
    {
        attackSpeed = value;
    }
    //public abstract void Death();
    //public abstract GameObject Attack();
}
[Serializable]
public class Boss : Enemy , IEnemy
{
    public GameObject uiParent;
    public GameObject health;
    public GameObject healthBossObj;
    public Image fillAmountImage;

    public ItemParameters itemPrefab;
    [HideInInspector]
    public List<SavedObjectData> itemsLoaded;
    public float spawnRare = 0.6f;
    public float spawnMiphical = 0.3f;
    public float spawnLegendary = 0.05f;

    public List<SavedObjectData> CommonItems;
    public List<SavedObjectData> RareItems;
    public List<SavedObjectData> MiphicalItems;
    public List<SavedObjectData> LegendaryItems;
    public bool isTutor;
    public string[] rarityType = { "Звичайне", "Рідкісне", "Міфічне", "Легендарне" };

    public GameObject SetBase()
    {
        uiParent = GameObject.Find("/UI");
        health = UnityEngine.Object.Instantiate(healthBossObj, uiParent.transform);
        health.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 2 - 100f, Screen.height / 2 - 130f);
        health.GetComponentInChildren<Image>().fillAmount = 1;
        fillAmountImage = health.GetComponentInChildren<Image>();
        health.SetActive(true);
        return healthBossObj;
    }

    public GameObject Attack(EnemyState enemy)
    {
        //throw new NotImplementedException();
        return enemy.objToAttack.gameObject;
    }
   
    public void Death(EnemyState enemy)
    {
        healthBossObj.SetActive(false);
        ItemRarity();
        float randomValue = UnityEngine.Random.value;

        List<SavedObjectData> rarityItems = GetRarityItems(randomValue);
        if (rarityItems != null)
            SetStats(rarityItems, isTutor, enemy.objTransform);
    }

    void SetStats(List<SavedObjectData> Rarity, bool isTutor,Transform objTransform)
    {
        int rand = UnityEngine.Random.Range(0, Rarity.Count);
        ItemParameters newItem = UnityEngine.Object.Instantiate(itemPrefab, objTransform.position, objTransform.rotation);
        newItem.itemName = Rarity[rand].Name;
        newItem.itemImage = Rarity[rand].ImageSprite;
        newItem.itemRareName = Rarity[rand].RareName;
        newItem.itemRare = Rarity[rand].RareSprite;
        newItem.idRare = Rarity[rand].IDRare;
        newItem.Stat = Rarity[rand].Stat;
        newItem.Level = Rarity[rand].Level;
        newItem.Count = Rarity[rand].Count;
        newItem.Tag = Rarity[rand].Tag;
        newItem.RareTag = Rarity[rand].RareTag;
        newItem.Description = Rarity[rand].Description;

        newItem.isTutor = isTutor;
    }

    List<SavedObjectData> GetRarityItems(float randomValue)
    {
        if (randomValue <= spawnLegendary)
            return LegendaryItems;
        else if (randomValue <= spawnMiphical)
            return MiphicalItems;
        else if (randomValue <= spawnRare)
            return RareItems;
        else if (randomValue <= 1)
            return CommonItems;

        return null;
    }

    void ItemRarity()
    {
        GameManager.Instance.LoadInventory(itemsLoaded);
        foreach (SavedObjectData line in itemsLoaded)
        {
            switch (line.RareName)
            {
                case "Звичайне":
                    CommonItems.Add(line);
                    break;
                case "Рідкісне":
                    RareItems.Add(line);
                    break;
                case "Міфічне":
                    MiphicalItems.Add(line);
                    break;
                case "Легендарне":
                    LegendaryItems.Add(line);
                    break;
            }
        }
    }
}
[Serializable]
public class Mele : Enemy, IEnemy
{
    public GameObject Attack(EnemyState enemy)
    {
        Shield shield = enemy.objectToHit.GetComponent<Shield>();
        if (shield != null)
        {
            shield.healthShield -= enemy.damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", enemy.damage);
        }
        else
        {
            PlayerManager.instance.TakeDamage(enemy.damage);
        }
        //throw new NotImplementedException();
        return enemy.objToAttack.gameObject;
    }
    public void Death(EnemyState enemy)
    {
        //throw new NotImplementedException();
        //return enemy.objToAttack.gameObject;
    }
}
[Serializable]
public class Range : Enemy, IEnemy
{
    public GameObject Attack(EnemyState enemy)
    {
        return enemy.objToAttack.gameObject;
        //throw new NotImplementedException();
    }
    public void Death(EnemyState enemy)
    {
        //throw new NotImplementedException();
        //return enemy.objToAttack.gameObject;
    }
}
[DefaultExecutionOrder(6)]
public class EnemyController : MonoBehaviour
{
    [field: SerializeField] public Boss bosses { get; set; }
    Mele mele { get; set; }
    Range range { get; set; }
    [field: SerializeField] public List<Enemy> enemies { get; private set; }
    [field: SerializeField] public List<EnemyPool> enemiesPool { get; private set; }
    [field: SerializeField] public float enemySpawnInterval { get; private set; }
    [field: SerializeField] public float timeStepWeed { get; private set; }
    [field: SerializeField] public Collider2D spawnMapBound { get; private set; }
    [field: SerializeField] public bool stopSpawn { get; private set; }
    [field: SerializeField] public Camera mainCamera { get; private set; }
    [field: SerializeField] public float spawnRadius { get; private set; }
    [field: SerializeField] public int enemycount { get; private set; }
    [field: SerializeField] public Transform parent { get; private set; }
    public Timer timer;
    public float timeToSpawnBobs;
    public bool isSpawned = false;
    float timeToSpawnBobsStart;

    GameManager gameManager;
    PlayerManager player;
    bool isTutorial;
    public Transform objTransform;
    [SerializeField] public List<EnemyState> children;
    Enemy matchingEnemy;
    public static EnemyController instance;
    [Header("Exp info")]
    public EXP expiriancePoint;
    GameObject objVFX;
    private void Awake()
    {
        instance ??= this;
    }
    public void SetSpawnStatus(bool currentStatus)
    {
        stopSpawn = currentStatus;
    }
    // Start is called before the first frame update
    void Start()
    {
        mele = new Mele();
        range = new Range();
        gameManager = GameManager.Instance;
        int buildIndex = gameManager.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex);
        if (!isTutorial)
        {
            foreach (var enemy in enemies)
            {
                if (buildIndex > 0)
                {
                    int firstLevelIndex = SceneManager.GetSceneByName("Level_1").buildIndex - 1;
                    float levelAceleration = buildIndex + SceneManager.GetActiveScene().buildIndex - firstLevelIndex;

                    enemy.SetSpeed((enemy.speedMax + levelAceleration) * 0.7f);
                    enemy.HealthChange((enemy.healthMax + levelAceleration) * 0.6f);
                    enemy.DamageReduce((enemy.damageMax + levelAceleration) * 1.2f);
                }
            }
            timeToSpawnBobs += (buildIndex + 1) * 15;

            timeToSpawnBobsStart = timeToSpawnBobs;
        }

        objTransform = transform;
        player = PlayerManager.instance;
        foreach (var enemyPool in enemiesPool)
        {
            enemyPool.enemyPool = InitializeObjectPool(enemies, enemyPool.enemyObj, enemyPool.poolSize, parent);
        }
        StartCoroutine(SpawnEnemyRoutine(enemySpawnInterval));
    }
   
    ///Spawn
    List<GameObject> InitializeObjectPool(List<Enemy> enemy, GameObject objectPrefab, int pool, Transform parent)
    {
        
        List<GameObject> objectPool = new List<GameObject>();
        for (int i = 0; i < pool; i++)
        {
            matchingEnemy = enemy.First(s => s.prefab.gameObject == objectPrefab);
           
            GameObject obj = Instantiate(objectPrefab, parent);
            obj.GetComponent<AIPath>().maxSpeed = matchingEnemy.speedMax;
            obj.GetComponent<EnemyState>().HealthDamage(matchingEnemy.healthMax);
            obj.GetComponent<EnemyState>().Damage(matchingEnemy.damageMax);
            obj.GetComponent<EnemyState>().SetAttackSpeed(matchingEnemy.attackSpeedMax);
            obj.gameObject.SetActive(false);
            obj.GetComponent<AIDestinationSetter>().target = player.objTransform;
            obj.GetComponent<EnemyState>().SetType(matchingEnemy.type);
            children.Add(obj.GetComponent<EnemyState>());
            objectPool.Add(obj);
        }
        return objectPool;
    }
    public void SpawnObjectInMapBounds(GameObject obj)
    {
        // Отримуємо центр колайдера
        Vector2 colliderCenter = spawnMapBound.bounds.center;
        Vector2 randomPointInsideCollider = colliderCenter + UnityEngine.Random.insideUnitCircle * new Vector2(spawnMapBound.bounds.size.x * 0.6f, spawnMapBound.bounds.size.y * 0.5f);
        obj.transform.position = randomPointInsideCollider;
    }
    GameObject GetFromPool(List<GameObject> objectPool)
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }
    private IEnumerator SpawnEnemyRoutine(float interval)
    {
        if (!stopSpawn)
        {
            for (int i = 0; i < enemiesPool.Count; i++)
            {
                for (int y = 0; y < enemiesPool[i].poolSize; y++)
                {
                    yield return new WaitForSeconds(interval);
                    if (children.Count > 0)
                    {
                        SpawnEnemies(enemiesPool[i].enemyPool, SpawnType.Camera, enemiesPool[i].enemyObj.GetComponent<EnemyState>().timesPerOne);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
    private bool IsInsideCameraBounds(Vector3 position)
    {
        Vector3 viewportPosition = GameManager.Instance.GetComponent<Camera>().WorldToViewportPoint(position);
        return viewportPosition.x >= 0f && viewportPosition.x <= 1f && viewportPosition.y >= 0f && viewportPosition.y <= 1f;
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

    public Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition;
        do
        {
            float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * spawnRadius;
            spawnPosition = new Vector3(GameManager.Instance.transform.position.x + spawnOffset.x, GameManager.Instance.transform.position.y + spawnOffset.y, 1.8f);
        } while (IsInsideCameraBounds(spawnPosition) || IsInsideWallBounds(spawnPosition));

        return spawnPosition;
    }

    private void SpawnEnemies(List<GameObject> pool, SpawnType type, int count)
    {
        GameObject enemy = GetFromPool(pool);
        // Отримати поточне ім'я об'єкта
        string name = enemy.name;

        // Видалити частину імені
        string newName = name.Replace("(Clone)", "");

        // Змінити ім'я об'єкта
        enemy.name = newName;

        IDChecker(enemy.name);
        for (int i = 0; i < count; i++)
        {
            switch (type)
            {
                case SpawnType.Camera:
                    float angle = i * Mathf.PI * 2 / 10; // Розраховуємо кут між об'єктами
                    Vector3 spawnPosition = GetRandomSpawnPosition();
                    spawnPosition += new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0); /** enemy.attackRangeRadius;*/ // Обчислюємо позицію для спавну
                    enemy.transform.position = spawnPosition;
                    //enemy.transform.parent = objTransform;
                    break;
                case SpawnType.Map:
                    SpawnObjectInMapBounds(enemy);
                    break;
            }
        }
        enemycount++;
    }
    public void IDChecker(string enemyName)
    {
        foreach (SaveEnemyInfo obj in gameManager.enemyInfo)
        {
            if (obj.Name.Contains(enemyName))
            {
                if (gameManager.CheckInfo(obj.ID))
                {
                    gameManager.FillInfo(obj.ID);
                    gameManager.enemyInfoLoad.Clear();
                    gameManager.LoadEnemyInfo();
                }
            }
        }
    }
    //Spawn end
    ///Move
    public IEnumerator SlowEnemy(EnemyState enemy, float time, float percent)
    {
        float speed = enemies.First(i => i.name == enemy.mobName).speedMax;
        enemy.GetComponent<AIPath>().maxSpeed = speed * percent;
        yield return new WaitForSeconds(time);
        enemy.GetComponent<AIPath>().maxSpeed = speed;
    }
    void EnemyRotate(EnemyState enemy)
    {
        Transform transform = enemy.transform;

        transform.rotation = transform.position.x < player.objTransform.position.x
            ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
    }
    //Move end
    // Update is called once per frame
    void FixedUpdate()
    {
        try
        {
            foreach (EnemyState enemy in children)
            {
                EnemyRotate(enemy);

                enemy.GetComponent<Animator>().enabled = IsInsideCameraBounds(player.objTransform.position) ? true : false;

                matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);
                enemy.GetComponent<AIPath>().maxSpeed = enemy.isSlowed ? 0 : matchingEnemy.speedMax;

                AttackSpeed(enemy);

                if (timer.time >= timeToSpawnBobs && isSpawned == false)
                {
                    BossSpawn();
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning("Catched exception modified array");
        }
    }
    ///Health
    public void Burn(EnemyState enemy, float burnTime, float burnTick, float burnDamage)
    {
        float burnTickMax = burnTick;
        matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);
        if (enemy.isBurn)
        {
            burnTime -= Time.fixedDeltaTime;
            if (burnTime <= 0)
            {
                enemy.SetBurn();
            }
            burnTick -= Time.fixedDeltaTime;
            if (burnTick <= 0)
            {
                enemy.HealthDamage(burnDamage);
                burnTick = burnTickMax;
            }
        }
        else
        {
            //End Fire Anim
            //ChangeToNotKick();
        }
    }
    void ExpGive(EnemyState enemy)
    {
        matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);
        EXP a = Instantiate(expiriancePoint, new Vector3(enemy.objTransform.position.x, enemy.objTransform.position.y, 1.9f), Quaternion.identity);
        a.expBuff = matchingEnemy.expGiven * matchingEnemy.dangerLevel;
        //if (player.expiriencepoint != null)
        {
            player.expiriencepoint.fillAmount += matchingEnemy.expGiven * matchingEnemy.dangerLevel / player.expNeedToNewLevel;
        }
        GameManager.Instance.score++;
    }
    public void Respawn(EnemyState enemy, SpawnType type)
    {
        switch (type)
        {
            case SpawnType.Camera:
                // Генеруємо випадкові координати в межах заданої області спавну
                Vector3 spawnPosition = GetRandomSpawnPosition();
                enemy.objTransform.position = spawnPosition;
                break;
            case SpawnType.Map:
                SpawnObjectInMapBounds(objTransform.parent.gameObject);
                break;
        }
        matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);
        Elements element = enemy.GetComponent<ElementActiveDebuff>().elements;
        for (int i = 0; i < element.isActiveCurrentData.Count; i++)
        {
            if (element.isActiveCurrentData[i])
            {
                element.DeactivateDebuff(enemy, (Elements.status)i, enemy.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.GetComponentInChildren<SpriteRenderer>().gameObject);
            }
        }
        enemy.HealthDamage(matchingEnemy.healthMax);
    }
    public void TakeDamage(EnemyState enemy, float damage)
    {
        enemy.HealthDamage(enemy.health - damage);
        matchingEnemy = bosses.prefab.mobName == enemy.mobName ? bosses : null;

        if (matchingEnemy != null)
        {
            Debug.Log(bosses.fillAmountImage);
            Debug.Log(bosses.healthMax);
            bosses.fillAmountImage.fillAmount -= damage / bosses.healthMax;
            if (enemy.health <= 0)
            {
                bosses.Death(enemy);
                Destroy(enemy.gameObject);
            }
        }
        else if (enemy.health <= 0)
        {
            ExpGive(enemy);
            GameManager.Instance.FindStatName(enemy.mobName, 1);

            Respawn(enemy, SpawnType.Camera);
        }
    }
    ///Attack
    public void AttackSpeed(EnemyState enemy)
    {
        if (enemy.attackSpeed <= 0)
        {
            if (enemy.objectToHit != null)
            {
                DamageDeal(enemy);
                Destroy(objVFX);
                enemy.SetAttackSpeed(matchingEnemy.attackSpeedMax);
            }
        }
        else
        {
            enemy.attackSpeed -= Time.fixedDeltaTime;
        }
    }
    public void DamageDeal(EnemyState enemy)
    {
        switch (enemy.type)
        {
            case "range":
                Instantiate(range.Attack(enemy), enemy.objTransform.position, Quaternion.identity);
                break;
            case "mele":
                Instantiate(mele.Attack(enemy), enemy.transform);
                break;
            case "boss":
                Instantiate(bosses.Attack(enemy), enemy.objTransform.position, Quaternion.identity);
                break;
        }
        ////if (enemy.objectToHit.GetComponent<Animator>())
        //{
        //    Shield shield = enemy.objectToHit.GetComponent<Shield>();
        //    if (shield != null)
        //    {
        //        shield.healthShield -= enemy.damage;
        //        GameManager.Instance.FindStatName("ShieldAbsorbedDamage", enemy.damage);
        //    }
        //    else
        //    {
        //        player.TakeDamage(enemy.damage);
        //    }
        //}
    }
    public void BossSpawn()
    {
        foreach (SaveEnemyInfo obj in gameManager.enemyInfo)
        {
            if (obj.Name.Contains(bosses.prefab.mobName))
            {
                if (gameManager.CheckInfo(obj.ID))
                {
                    gameManager.FillInfo(obj.ID);
                    gameManager.enemyInfoLoad.Clear();
                    gameManager.LoadEnemyInfo();
                }
            }
        }
        bosses.healthBossObj = bosses.SetBase();

        //GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("Enemy");

        //foreach (var obj in objectsToDelete)
        //{
        //    Destroy(obj.GetComponentInParent<HealthPoint>().transform.parent.gameObject);
        //    gameManager.enemyCount = 0;
        //}
        children.Clear();
        parent.gameObject.SetActive(false);
        GetComponent<EnemyController>().SetSpawnStatus(true);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic("BossFight1");
        }
        GameObject boss = Instantiate(bosses.prefab.gameObject, transform.position, Quaternion.identity);
        boss.GetComponent<AIPath>().maxSpeed = bosses.speedMax;
        boss.GetComponent<EnemyState>().HealthDamage(bosses.healthMax);
        boss.GetComponent<EnemyState>().Damage(bosses.damageMax);
        boss.GetComponent<EnemyState>().SetAttackSpeed(bosses.attackSpeedMax);
        boss.GetComponent<AIDestinationSetter>().target = player.objTransform;
        boss.GetComponent<EnemyState>().SetType(bosses.type);

        //boss.GetComponent<Forward>().player = player;
        isSpawned = true;
        timeToSpawnBobs += timeToSpawnBobsStart;
    }
    //Attack end
}
//public void SetOthwerPartsCount(EnemyState enemy)
//{
//    // Знаходимо всі об'єкти в сцені з компонентом MyComponent
//    HealthPoint[] objects = UnityEngine.Object.FindObjectsOfType<HealthPoint>();

//    // Проходимося по знайдених об'єктах
//    foreach (HealthPoint obj in objects)
//    {
//        CutThePart part = obj.GetComponent<CutThePart>();
//        // Перевіряємо значення булевої змінної у кожному об'єкті
//        if (obj.isBossPart)
//        {
//            if (part.countParts == 1)
//            {
//                Death(enemy);
//                //objDrop.OnDestroyBoss();
//            }
//            part.countParts--;
//        }
//    }
//}
//Health end
///Рух кожної частини босса до гравця по траекторії схожої на колесо
//    // визначаємо напрямок до гравця
//    Vector2 direction = player.transform.position - objTransform.position;
//    direction.Normalize();

//            // визначаємо кут між напрямком до гравця і поточним напрямком руху об'єкту
//            angle = Vector2.Angle(velocity, direction);

//            // якщо кут між векторами більший за 20 градусів - зменшуємо швидкість
//            if (angle > 20f)
//            {
//                // зменшуємо швидкість з поступовим нарощуванням
//                velocity = Vector2.Lerp(velocity, direction* speedMax, Time.fixedDeltaTime* acceleration);
//            }
//            else // якщо кут менший - збільшуємо швидкість
//{
//    // збільшуємо швидкість з поступовим нарощуванням
//    velocity = Vector2.Lerp(velocity, direction * speedMax, Time.fixedDeltaTime * acceleration);
//}

//// обмежуємо максимальну швидкість
//velocity = Vector2.ClampMagnitude(velocity, speedMax);

//// зміщуємо об'єкт на відстань, що дорівнює швидкості, помноженій на час оновлення
//objTransform.Translate(velocity * Time.fixedDeltaTime);

//if (isBossPart)
//{
//    SetOtherPartsCount();
//    //ExpGive();
//    Destroy(gameObject);
//}


///Boss Destroy 
//if (IsBobs)
//{
//    if (isBossPart)
//    {
//        SetOtherPartsCount();
//        //ExpGive();
//        Destroy(gameObject);
//    }
//    else if (bodyAnimBoss != null && !AnimBossStart)
//    {
//        player.expiriencepoint.fillAmount += expGiven * dangerLevel / player.expNeedToNewLevel;
//        GameManager.Instance.score++;
//        foreach (var part in bodyAnimBoss.parts)
//        {
//            if (part == gameObject.name)
//            {
//                bodyAnimBoss.GetParts(objPart, healthPointMax);
//                bodyAnimBoss.DestroyStart();
//            }
//        }
//        AnimBossStart = true;
//        bodyAnimBoss = null;
//    }
//    else if (!AnimBossStart)
//    {
//        objDrop.OnDestroyBoss(health);
//        //ExpGive();
//        Destroy(objTransform.root.gameObject);
//    }
//}

///SetParentToBossesParts
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

