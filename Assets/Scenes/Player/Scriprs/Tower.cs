using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float lifeTime;
    public float spawnTick;
    public float spawnTickMax;
    public TowerWave damageObj;
    public GameObject bomb;
    public bool isThree;
    public bool isFive;
    Collider2D[] colliders;
    float agreTime = 3;
    Forward objEnemyMove;
    // Start is called before the first frame update
    void Start()
    {

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
            a.player = gameObject;
            spawnTick = spawnTickMax;
        }
    }
    private IEnumerator TimerLife()
    {
        yield return new WaitForSeconds(lifeTime);
        if (isThree)
        {
            Instantiate(bomb, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    private IEnumerator TimerAgre(Forward a)
    {
        yield return new WaitForSeconds(agreTime);
        if (a != null)
        {
            a.player = FindObjectOfType<Forward>().gameObject;
            a.enemyFinded = false;
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
                        collider.transform.root.GetComponent<Forward>().player = gameObject;
                        collider.transform.root.GetComponent<Forward>().enemyFinded = true;
                        objEnemyMove = collider.transform.root.GetComponent<Forward>();
                    }
                }
            }
        }
    }
}
