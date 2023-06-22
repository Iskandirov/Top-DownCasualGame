using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class LightOn : MonoBehaviour
{
    [Header("Check if pill eated")]
    //Check if pill eated
    public bool IsPillUp;
    public bool IsPillBuffed;

    [Header("GameObjects")]
    //GameObjects
    public GameObject pill;
    public GameObject spawner;
    Move objMove;
    Shoot objShoot;
    SpawnEnemy objSpawn;

    [Header("Check if skill activated")]
    //Check if skill activated
    public bool IsPillOn;
    public bool IsLightOn;
    public bool IsPuddletOn;

    [Header("Timer for pills")]
    //Timer for pills
    public float step;
    public float stepMax;

    [Header("Timer for ghost enemies")]
    //Timer for ghost enemies
    public float stepGhost;
    public float stepGhostMax;


    public float attackSpeedBuff;
    public float moveSpeedBuff;
    public float dashTime;

    public Vector2 spawnAreaMin; // Мінімальні координати спавну
    public Vector2 spawnAreaMax; // Максимальні координати спавну

    ElementsCoeficients grassWindElement;
    // Start is called before the first frame update
    void Start()
    {
        grassWindElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
        objMove = transform.root.GetComponent<Move>();
        objShoot = transform.root.GetComponent<Shoot>();
        objSpawn = FindObjectOfType<SpawnEnemy>();
    }

    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
        {
            SpawnObject();
            step = stepMax;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Spawn ghost enemies while pill is active
        if (IsPillUp == true)
        {
            stepGhost -= Time.deltaTime;
            objSpawn.SpawnEnemies(120, moveSpeedBuff, 1, 1.3f);
            if (!IsPillBuffed)
            {
                objMove.speed += moveSpeedBuff * grassWindElement.Grass;
                objShoot.attackSpeed -= attackSpeedBuff * grassWindElement.Wind / 100;
                IsPillBuffed = true;
                objMove.dashTimeMax *= dashTime;
            }

            if (stepGhost <= 0)
            {
                stepGhost = stepGhostMax;
                objMove.speed -= moveSpeedBuff * grassWindElement.Grass;
                objShoot.attackSpeed += attackSpeedBuff * grassWindElement.Wind / 100;
                objMove.dashTimeMax /= dashTime;
                IsPillBuffed = false;
                IsPillUp = false;
            }
        }
    }
    private void SpawnObject()
    {
        // Генеруємо випадкові координати в межах заданої області спавну
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        // Створюємо об'єкт з використанням префабу і випадкових координат
        Instantiate(pill, new Vector3(randomX, randomY, 0f), Quaternion.identity);
    }
}
