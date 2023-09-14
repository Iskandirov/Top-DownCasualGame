using System.Collections.Generic;
using UnityEngine;

public class SpawnBobs : MonoBehaviour
{
    Timer timer;
    public float timeToSpawnBobs;
    float timeToSpawnBobsStart;
    public GameObject bobs;
    public int bosscount;
    public bool isSpawned = false;
    public KillCount countEnemy;
    public EnemyInfoLoader enemyInfo;
    // Start is called before the first frame update
    void Start()
    {
        timeToSpawnBobsStart = timeToSpawnBobs;
        timer = FindObjectOfType<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.time >= timeToSpawnBobs && isSpawned == false)
        {
            foreach (SaveEnemyInfo obj in enemyInfo.enemyInfo)
            {
                if (obj.Name.Contains(bobs.name))
                {
                    if (enemyInfo.CheckInfo(obj.ID))
                    {
                        enemyInfo.FillInfo(obj.ID);
                        enemyInfo.enemyInfoLoad.Clear();
                        enemyInfo.LoadEnemyInfo();
                    }
                }
            }
            
            GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var obj in objectsToDelete)
            {
                Destroy(obj.GetComponentInParent<HealthPoint>().transform.parent.gameObject);
                countEnemy.enemyCount = 0;
            }
            GameObject spawners = GameObject.FindGameObjectWithTag("Spawner");
            spawners.GetComponent<SpawnEnemy>().stopSpawn = true;

            Instantiate(bobs, transform.position, Quaternion.identity);
            isSpawned = true;
            timeToSpawnBobs += timeToSpawnBobsStart;
        }
    }
}
