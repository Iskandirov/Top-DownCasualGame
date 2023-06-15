using System.Collections.Generic;
using UnityEngine;

public class SpawnBobs : MonoBehaviour
{
    Timer timer;
    public float timeToSpawnBobs;
    float timeToSpawnBobsStart;
    public List<GameObject> bobs;
    public int bosscount;
    private GameObject bossRandomed;
    public bool isSpawned = false;
    // Start is called before the first frame update
    void Start()
    {
        timeToSpawnBobsStart = timeToSpawnBobs;
        timer = FindObjectOfType<Timer>();
    }

    public void RandomBoss()
    {
        bosscount = Random.Range(0, bobs.Count);
        bossRandomed = bobs[bosscount];
    }
    // Update is called once per frame
    void Update()
    {
        if (timer.time >= timeToSpawnBobs && isSpawned == false)
        {
            RandomBoss();
            GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var obj in objectsToDelete)
            {
                Destroy(obj.GetComponentInParent<Forward>().gameObject);
            }
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");

            foreach (var obj in spawners)
            {
                obj.GetComponent<SpawnEnemy>().stopSpawn = true;
            }
            Instantiate(bossRandomed, transform.position, Quaternion.identity);
            isSpawned = true;
            timeToSpawnBobs += timeToSpawnBobsStart;
        }
    }
}
