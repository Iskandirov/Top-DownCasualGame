using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [field: SerializeField] public GameObject attackPrefab { get; private set; }
    public Enemy()
    {
        health = healthMax;
        damage = damageMax;
        attackSpeed = attackSpeedMax;
        maxCountPerTime = 1;
    }
    public interface IAttackable
    {
        void Attack();
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
public class Boss : Enemy , IAttackable
{
    GameObject health;
    public GameObject uiParent;
    public GameObject healthBossObj;
    public void SetBase()
    {
        uiParent = GameObject.Find("/UI");
       
    }

    public void Attack()
    {
        //throw new NotImplementedException();
    }
    //public override GameObject Attack()
    //{
    //    return attackPrefab;
    //}
    //public override void Death()
    //{
    //}
}
[Serializable]
public class Mele : Enemy, IAttackable
{
    public void Attack()
    {
        DamageReduce(1);
        //throw new NotImplementedException();
    }
    //public override GameObject Attack()
    //{
    //    return attackPrefab;
    //}
    //public override void Death()
    //{
    //}
}
[Serializable]
public class Range : Enemy, IAttackable
{
    public void Attack()
    {
        //throw new NotImplementedException();
    }
    //public override GameObject Attack()
    //{
    //    return attackPrefab;
    //}
    //public override void Death()
    //{
    //}
}
[DefaultExecutionOrder(6)]
public class EnemyController : MonoBehaviour
{
    [field: SerializeField] public List<Boss> bosses { get; private set; }
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
    GameManager gameManager;
    PlayerManager player;
    bool isTutorial;
    public Transform objTransform;
    [SerializeField] public List<EnemyState> children;
    Enemy matchingEnemy;
    public static EnemyController instance;
    [Header("Exp info")]
    public float expGiven;
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
        if (!isTutorial)
        {
            foreach (var enemy in enemies)
            {
                if (GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) > 0)
                {
                    enemy.SetSpeed((enemy.speedMax + GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) ) * 1.1f);
                    enemy.HealthChange((enemy.healthMax + GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex)) * 1.3f);
                    enemy.DamageReduce((enemy.damageMax + GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex)) * 1.3f);
                }
            }
        }
        //health = Instantiate(healthBossObj, uiParent.transform);
        //health.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 2 - 100f, Screen.height / 2 - 130f);
        //health.GetComponentInChildren<Image>().fillAmount = 1;
        objTransform = transform;
        gameManager = GameManager.Instance;
        player = PlayerManager.instance;
        //foreach (var enemyPool in enemiesPool)
        {
            foreach (var enemyPool in enemiesPool)
            {
                enemyPool.enemyPool = InitializeObjectPool(enemies, enemyPool.enemyObj, enemyPool.poolSize, parent);
            }
        }
            StartCoroutine(SpawnEnemyRoutine(enemySpawnInterval));
    }
    ///Spawn
    List<GameObject> InitializeObjectPool(IEnumerable<Enemy> enemy, GameObject objectPrefab, int pool, Transform parent)
    {
        Enemy mele = new Mele();
        mele.att
        List<GameObject> objectPool = new List<GameObject>();
        for (int i = 0; i < pool; i++)
        {
            GameObject obj = Instantiate(objectPrefab, parent);

            obj.GetComponent<AIPath>().maxSpeed = enemy.First(s => s.prefab.gameObject == objectPrefab).speedMax;
            obj.GetComponent<EnemyState>().HealthDamage(enemy.First(s => s.prefab.gameObject == objectPrefab).healthMax);
            obj.GetComponent<EnemyState>().Damage(enemy.First(s => s.prefab.gameObject == objectPrefab).damageMax);
            obj.GetComponent<EnemyState>().SetAttackSpeed(enemy.First(s => s.prefab.gameObject == objectPrefab).attackSpeedMax);
            obj.gameObject.SetActive(false);
            obj.GetComponent<AIDestinationSetter>().target = player.objTransform;
           
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
                   
                    SpawnEnemies(enemiesPool[i].enemyPool, SpawnType.Camera, enemiesPool[i].enemyObj.GetComponent<EnemyState>().timesPerOne);
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
                    enemy.transform.parent = objTransform;
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
        foreach (EnemyState enemy in children)
        {
            EnemyRotate(enemy);

            enemy.GetComponent<Animator>().enabled = IsInsideCameraBounds(player.objTransform.position) ? true : false;

            matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);
            enemy.GetComponent<AIPath>().maxSpeed = enemy.isSlowed ? 0 : matchingEnemy.speedMax;

            if (enemy.attackSpeed <= 0)
            {
                if (enemy.objectToHit != null && enemy.isAttack == false)
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
        a.expBuff = expGiven * matchingEnemy.dangerLevel;

        player.expiriencepoint.fillAmount += expGiven * matchingEnemy.dangerLevel / player.expNeedToNewLevel;
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
        enemy.HealthDamage(matchingEnemy.healthMax);
    }
    public void TakeDamage(EnemyState enemy, float damage)
    {
        enemy.HealthDamage(enemy.health - damage);
        matchingEnemy = bosses.FirstOrDefault(s => s.prefab.mobName == enemy.mobName);

        if (matchingEnemy != null)
        {
            //GetComponentInChildren<Image>().fillAmount -= damage / healthPointMax;
        }
        if (enemy.health <= 0)
        {
            ExpGive(enemy);
            Respawn(enemy, SpawnType.Camera);

            GameManager.Instance.FindStatName(enemy.mobName, 1);
        }

    }
    ///Attack
    public void DamageDeal(EnemyState enemy)
    {
        //if (enemy.objectToHit.GetComponent<Animator>())
        {
            Shield shield = enemy.objectToHit.GetComponent<Shield>();
            if (shield != null)
            {
                shield.healthShield -= enemy.damage;
                GameManager.Instance.FindStatName("ShieldAbsorbedDamage", enemy.damage);
            }
            else
            {
                player.TakeDamage(enemy.damage);
            }
        }
    }
   
    //Attack end
}
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

