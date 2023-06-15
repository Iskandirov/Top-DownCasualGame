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
        enemy.GetComponentInParent<Forward>().isSummoned = true;
        enemy.GetComponentInParent<Forward>().summonTime = lifeTime;
        enemy.GetComponentInParent<Forward>().isThree = isThree;
        enemy.GetComponentInParent<Forward>().bomb = bomb.gameObject;
        enemy.GetComponentInParent<Attack>().isFive = isFive;
    }
    void Update()
    {
        transform.position = player.transform.position;
    }
}
