using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnBobs : MonoBehaviour
{
    GameObject timer;
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
        timer = GameObject.Find("Player/UI/Timer");
    }

    public void RandomBoss()
    {
        bosscount = Random.Range(0, bobs.Count);
        bossRandomed = bobs[bosscount];
    }
    // Update is called once per frame
    void Update()
    {
        if (timer.GetComponent<Timer>().time >= timeToSpawnBobs && isSpawned == false)
        {
            RandomBoss();
            Instantiate(bossRandomed, transform.position, Quaternion.identity);
            isSpawned = true;
            timeToSpawnBobs += timeToSpawnBobsStart;
        }
    }
}
