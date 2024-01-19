using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : SkillBaseMono
{
    public float spawnTick;
    public float spawnTickMax;
    public TowerWave damageObj;
    public BobmExplode bomb;
    public bool isThree;
    public bool isFive;
    Collider2D[] colliders;
    float agreTime = 3;
    Forward objEnemyMove;
    public float waterElement;
    public float fireElement;
    
    // Start is called before the first frame update
    void Start()
    {
        //basa = SetToSkillID(gameObject);
        waterElement = PlayerManager.instance.Water;
        spawnTickMax = basa.damageTickMax;
        fireElement = PlayerManager.instance.Fire;
        StartCoroutine(TimerSpell());
        StartCoroutine(TimerLife());
        StartCoroutine(TimerAgre(objEnemyMove));
    }
    private IEnumerator TimerSpell()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTick);
            TowerWave a = Instantiate(damageObj, transform.position, Quaternion.identity);
            a.waterElement = waterElement;
            spawnTick = spawnTickMax;
        }
    }
    private IEnumerator TimerLife()
    {
        yield return new WaitForSeconds(basa.lifeTime);
        if (isThree)
        {
            BobmExplode a = Instantiate(bomb, transform.position, Quaternion.identity);
            a.fire = fireElement;
        }
        Destroy(gameObject);
    }
    private IEnumerator TimerAgre(Forward a)
    {
        yield return new WaitForSeconds(agreTime);
        if (a != null)
        {
            a.SetDestination(PlayerManager.instance.gameObject);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFive)
        {
            colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 16f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.isTrigger != true && collider.CompareTag("Enemy") && collider.transform.parent.gameObject != gameObject)
                {
                    if (collider.GetComponent<HealthPoint>())
                    {
                        collider.transform.root.GetComponent<Forward>().SetDestination(gameObject);
                        objEnemyMove = collider.transform.root.GetComponent<Forward>();
                    }
                }
            }
        }
    }
}
