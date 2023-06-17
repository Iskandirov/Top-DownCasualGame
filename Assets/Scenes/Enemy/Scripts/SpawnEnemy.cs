using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public float timeStep;
    public float timeToNewType;
    public float timeToNewTypeStart;
    public int enemyTypeSpawn;
    [SerializeField]float time;
    [SerializeField]float timeGost;
    public GameObject[] EnemyBody;
    public Timer Timer;
    public bool stopSpawn;
    public KillCount countEnemy;
    Vector2 spawnAreaMin; // Мінімальні координати спавну
    Vector2 spawnAreaMax; // Максимальні координати спавну
    // Start is called before the first frame update
    void Start()
    {
        spawnAreaMin = new Vector2(-199, -100);
        spawnAreaMax = new Vector2(220, 220);

        timeToNewType = timeToNewTypeStart;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemies();
    }
    public void SpawnEnemies()
    {
        time += Time.deltaTime;
        if (time >= timeStep && !stopSpawn && countEnemy.enemyCount <= 100)
        {
            int i = Random.Range(0, enemyTypeSpawn);
            Instantiate(EnemyBody[i], new Vector3(Random.Range(spawnAreaMin.x, spawnAreaMax.x), Random.Range(spawnAreaMin.y, spawnAreaMax.y), 1.8f), Quaternion.identity, transform);
            countEnemy.enemyCount += 1;
            time = 0;
        }
        if (Timer.time >= timeToNewType && enemyTypeSpawn < EnemyBody.Length)
        {
            timeToNewType += timeToNewTypeStart;
            enemyTypeSpawn++;
        }
    }
    public void SpawnEnemies(byte opacity,float speed,int health,float damage)
    {
        timeGost += Time.deltaTime;
        if (timeGost >= timeStep)
        {
            int i = Random.Range(0, EnemyBody.Length);
            GameObject enemy = Instantiate(EnemyBody[i], new Vector3(gameObject.transform.position.x + Random.Range(-30, 30), gameObject.transform.position.y + Random.Range(-30, 30), 1.8f), Quaternion.identity);
            enemy.GetComponentInChildren<SpriteRenderer>().color = new Color32(255, 255, 255, opacity);
            enemy.GetComponent<Forward>().speed *= speed;
            enemy.GetComponentInChildren<HealthPoint>().healthPoint = health;
            enemy.GetComponent<Attack>().damage *= damage;
           
            timeGost = 0;
        }
    }
}
