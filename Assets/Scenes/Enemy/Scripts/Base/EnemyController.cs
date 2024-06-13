using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
    [field: SerializeField] public bool isSpawnInsideMapBounds { get; private set; }
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
}
[Serializable]
public class Boss : Enemy , IEnemy
{
    public GameObject health;
    public Image fillAmountImage;
    public SpriteRenderer Avatar;
    public Sprite AvatarSprite;

    public bool isTutor;
    public GameObject SetBase()
    {
        health.SetActive(true);
        fillAmountImage = health.GetComponentInChildren<Image>();
        fillAmountImage.fillAmount = 1;
        Avatar.sprite = AvatarSprite;
        return health;
    }

    public GameObject Attack(EnemyState enemy)
    {
        //throw new NotImplementedException();
        return enemy.AttackObj.gameObject;
    }
   
    public void Death(EnemyState enemy)
    {
        health.SetActive(false);
        LootManager.inst.DropLoot(isTutor, enemy.objTransform);
       
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
        return enemy.AttackObj.gameObject;
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
        return enemy.AttackObj.gameObject;
        //throw new NotImplementedException();
    }
    public void Death(EnemyState enemy)
    {
        //throw new NotImplementedException();
        //return enemy.objToAttack.gameObject;
    }
}
//[DefaultExecutionOrder(6)]
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

    GameManager gameManager;
    PlayerManager player;
    bool isTutorial;
    public Transform objTransform;
    public List<EnemyState> children;
    Enemy matchingEnemy;
    public static EnemyController instance;
    [Header("Exp info")]
    public EXP expiriancePoint;
    GameObject objVFX;
    private void Awake()
    {
        instance = this;
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
        int LevelID = gameManager.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex);
        string path = "Assets/Scenes/Level_2.unity";
        int index =  SceneUtility.GetBuildIndexByScenePath(path);


        if (!isTutorial)
        {
            foreach (var enemy in enemies)
            {
                if (LevelID > 0)
                {
                    int firstLevelIndex = index - 1;
                    float levelAceleration = LevelID + SceneManager.GetActiveScene().buildIndex - firstLevelIndex;
                    enemy.SetSpeed((enemy.speedMax * levelAceleration * 0.33f) * 0.7f);
                    enemy.HealthChange((enemy.healthMax + levelAceleration) * 0.6f);
                    enemy.DamageReduce((enemy.damageMax + levelAceleration) * 1.2f);
                }
            }
            timeToSpawnBobs += (LevelID + 1) * 15;

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
    static public bool GetSpawnPositionNotInAIPath(float radius, Vector2 pos)
    {
        // Створити список доступних позицій
        List<Vector3> availablePositions = new List<Vector3>();

        // Отримати вузли сітки
        var nodes = AstarPath.active.data.gridGraph.nodes;

        // Перебрати всі вузли в радіусі
        foreach (var node in nodes.Where(n => Mathf.Abs(n.position.x) > radius || Mathf.Abs(n.position.y) > radius))
        {
            if (node.Walkable)
            {
                availablePositions.Add(new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid));
                Debug.Log(new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid));
            }
        }

        return availablePositions.Contains(pos);
    }
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
    public Vector3 SpawnObjectInMapBounds()
    {
        // Отримуємо центр колайдера
        Vector2 colliderCenter = spawnMapBound.bounds.center;
        Vector2 randomPointInsideCollider = colliderCenter + UnityEngine.Random.insideUnitCircle * new Vector2(spawnMapBound.bounds.size.x * 0.6f, spawnMapBound.bounds.size.y * 0.5f);
        return randomPointInsideCollider;
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
                        matchingEnemy = enemies.First(s => s.prefab.mobName == enemiesPool[i].enemyObj.GetComponent<EnemyState>().mobName);
                        SpawnEnemies(enemiesPool[i].enemyPool, matchingEnemy.isSpawnInsideMapBounds, enemiesPool[i].enemyObj.GetComponent<EnemyState>().timesPerOne);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
    //Перевірка чи об'єкт за межами камери
    private bool IsInsideCameraBounds(Vector3 position,bool needToBeOutside)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
        if (needToBeOutside)
            return viewportPosition.x >= 0f && viewportPosition.x <= 1f && viewportPosition.y >= 0f && viewportPosition.y <= 1f;
        else
            return viewportPosition.x <= 0.2f && viewportPosition.x >= 0.8f && viewportPosition.y <= 0.2f && viewportPosition.y >= 0.8f;
    }
    //Перевірка чи об'єкт не за межами стін
    private bool IsInsideWallBounds(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
    /*Спавн за межами камери*/
    public Vector3 GetRandomSpawnPosition(Vector3 pos, bool needToBeOutside,float radius)
    {
        Vector3 spawnPosition;
        do
        {
            float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * radius;
            spawnPosition = new Vector3(pos.x + spawnOffset.x, pos.y + spawnOffset.y, 0);
        } while (IsInsideCameraBounds(spawnPosition, needToBeOutside) || IsInsideWallBounds(spawnPosition));

        return spawnPosition;
    }

    private void SpawnEnemies(List<GameObject> pool, bool spawnChoise, int count)
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
            switch (spawnChoise)
            {
                case false:
                    float angle = i * Mathf.PI * 2 / 10; // Розраховуємо кут між об'єктами
                    Vector3 spawnPosition = GetRandomSpawnPosition(mainCamera.transform.position,true,spawnRadius);
                    spawnPosition += new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0); /** enemy.attackRangeRadius;*/ // Обчислюємо позицію для спавну
                    enemy.transform.position = spawnPosition;
                    break;
                case true:
                    enemy.transform.position = SpawnManager.GetRandomPositionInsideCollider();
                    enemy.GetComponent<EnemyState>().path.maxSpeed = 0;
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
    public IEnumerator FreezeEnemy(EnemyState enemy)
    {
        enemy.isFreezed = true;
        enemy.path.maxSpeed = 0;
        yield return new WaitForSeconds(5f);
        enemy.isFreezed = false;
        matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);
        enemy.path.maxSpeed = matchingEnemy.speedMax;
    }
    public IEnumerator SlowEnemy(EnemyState enemy, float time, float percent)
    {
        float speed = enemies.First(i => i.name == enemy.mobName).speedMax;
        enemy.GetComponent<AIPath>().maxSpeed = speed * percent;
        yield return new WaitForSeconds(time);
        enemy.GetComponent<AIPath>().maxSpeed = speed;
    }
    void EnemyRotate(EnemyState enemy)
    {
        if (!enemy.isStun)
        {
            Transform transform = enemy.transform;

            transform.rotation = transform.position.x < player.objTransform.position.x
                ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        }
    }
    //Move end
    // Update is called once per frame
    void FixedUpdate()
    {
        try
        {
            foreach (EnemyState enemy in children)
            {
                if (!enemy.isFreezed)
                {
                    EnemyRotate(enemy);
                    enemy.GetComponent<Animator>().enabled = IsInsideCameraBounds(player.objTransform.position,true) ? true : false;
                    matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);
                    MoveAndAttack(enemy);

                    if (timer.time >= timeToSpawnBobs && isSpawned == false)
                    {
                        BossSpawn();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e + " _Catched exception modified array");
        }
    }
    private void MoveAndAttack(EnemyState enemy)
    {
        if (enemy.type == "range" && enemy.isActiveAndEnabled && enemy.path.maxSpeed > 0)
        {
            float distanceToPlayer = Vector3.Distance(enemy.objTransform.position, player.objTransform.position);
            float distanceToEdgeAttack = 59f;
            float distanceToEdgeRetreat = 30f;

            if (distanceToPlayer <= distanceToEdgeAttack)
            {
                AttackSpeed(enemy);
                enemy.SetStunned();
            }
            if (distanceToPlayer <= distanceToEdgeRetreat)
            {
                enemy.RepositionPoint.transform.position = (player.objTransform.position - enemy.objTransform.position).normalized;
                enemy.destination.target = enemy.RepositionPoint.transform;
            }
            else
            {
                enemy.destination.target = player.objTransform;
                enemy.path.maxSpeed = enemy.isSlowed ? 0 : matchingEnemy.speedMax;
            }
        }
        else if(enemy.isActiveAndEnabled)
        {
            enemy.path.maxSpeed = enemy.isSlowed ? 0 : matchingEnemy.speedMax;

            AttackSpeed(enemy);
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
    void ExpGive(EnemyState enemy, Vector3 pos)
    {
        matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);
        EXP a = Instantiate(expiriancePoint, pos, Quaternion.identity);
        a.expBuff = matchingEnemy.expGiven * matchingEnemy.dangerLevel;
        PlayerManager.instance.expiriencepoint.fillAmount += matchingEnemy.expGiven * matchingEnemy.dangerLevel / PlayerManager.instance.expNeedToNewLevel;
        GameManager.Instance.score++;
    }
    public void Respawn(EnemyState enemy, SpawnType type)
    {
        switch (type)
        {
            case SpawnType.Camera:
                // Генеруємо випадкові координати в межах заданої області спавну
                Vector3 spawnPosition = GetRandomSpawnPosition(mainCamera.transform.position, true, spawnRadius);
                enemy.objTransform.position = spawnPosition;
                break;
            case SpawnType.Map:
                objTransform.parent.transform.position = SpawnObjectInMapBounds();
                break;
        }
        matchingEnemy = enemies.First(s => s.prefab.mobName == enemy.mobName);

        /*Видалення  активних дебаффів*/
        Elements element = enemy.GetComponent<ElementActiveDebuff>().elements;
        for (int i = 0; i < element.isActiveCurrentData.Count; i++)
        {
            if (element.isActiveCurrentData[i])
            {
                element.DeactivateDebuff(enemy, (Elements.status)i);
            }
        }
        for (int i = 0; i < enemy.ElementsParent.transform.childCount; i++)
        {
            Transform child = enemy.ElementsParent.transform.GetChild(i);

            Destroy(child.gameObject);
        }

        enemy.HealthDamage(matchingEnemy.healthMax);
    }
    public void TakeDamage(EnemyState enemy, float damage)
    {
        if (enemy.GetComponentInChildren<SpriteRenderer>().color.a != 0)
        {
            enemy.HealthDamage(enemy.health - damage);

            matchingEnemy = bosses.prefab.mobName == enemy.mobName ? bosses : null;
            if (matchingEnemy != null)
            {
                bosses.fillAmountImage.fillAmount -= damage / bosses.healthMax;
                if (enemy.health <= 0)
                {
                    if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 0 && s.isActive == true) != null)
                    {
                        DailyQuests.instance.UpdateValue(0, 1, false);
                    }
                    bosses.Death(enemy);
                    children.Clear();
                    Destroy(enemy.gameObject);
                }
            }
            else if (enemy.health <= 0)
            {
                if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 0 && s.isActive == true) != null)
                {
                    DailyQuests.instance.UpdateValue(0, 1, false);
                }
                if (enemy.mobName.Equals("MiShroom") && DailyQuests.instance.quest.FirstOrDefault(s => s.id == 2 && s.isActive == true) != null)
                {
                    DailyQuests.instance.UpdateValue(2, 1, false);
                }
                GameManager.Instance.FindStatName(enemy.mobName, 1);
                Vector3 pos = enemy.transform.position;
                Respawn(enemy, SpawnType.Camera);
                ExpGive(enemy, pos);
            }
        }
    }
    ///Attack
    public void AttackSpeed(EnemyState enemy)
    {
        if (enemy.attackSpeed <= 0)
        {
            if (enemy.objectToHit != null && enemy.isAttack)
            {
                enemy.SetAttackSpeed(matchingEnemy.attackSpeedMax);
                DamageDeal(enemy);
                Destroy(objVFX);
            }
        }
        enemy.attackSpeed -= Time.fixedDeltaTime;
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
    }
    //Attack end
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
        bosses.health = bosses.SetBase();


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
        children.Add(boss.GetComponent<EnemyState>());
        isSpawned = true;

    }
}

