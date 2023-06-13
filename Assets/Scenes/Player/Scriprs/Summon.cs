using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public float step;
    public float stepMax;
    public GameObject player;
    public GameObject enemy;
    public BobmExplode bomb;
    public float lifeTime;
    public bool isThree;
    public float attackSpeed;
    public bool isFive;

    private void Start()
    {
        bomb.lifeTime = 0.1f;
        player = FindObjectOfType<Move>().gameObject;
        enemy.transform.root.GetComponent<Forward>().isSummoned = true;
        enemy.transform.root.GetComponent<Forward>().summonTime = lifeTime;
        enemy.transform.root.GetComponent<Forward>().isThree = isThree;
        enemy.transform.root.GetComponent<Forward>().bomb = bomb.gameObject;
        enemy.transform.root.GetComponent<Attack>().isFive = isFive;
    }
    void Update()
    {
        transform.position = player.transform.position;
    }
}
