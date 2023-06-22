using System.Collections;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public Collider2D[] enemies;
    public Collider2D[] enemiesToShoot;
    public GameObject lightObj;
    public float radius;
    HealthPoint objHealth;
    HealthBossPart objHealthB;
    Forward objMove;
    public float step;
    public float stepMax;
    public int maxEnemiesToShoot;
    public float stunTime;
    public float damage;
    public float spawnInterval;
    int countEnemy;
    ElementsCoeficients ElectricityElement;
    // Start is called before the first frame update
    void Start()
    {
        ElectricityElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
    }

    void Update()
    {
        step -= Time.deltaTime;
        if (step <= 0)
        {
            enemiesToShoot = new Collider2D[maxEnemiesToShoot];
            enemies = Physics2D.OverlapCircleAll(gameObject.transform.position, radius);

            if (enemies != null && enemies.Length > 0)
            {
                int enemiesCount = 0;
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].isTrigger != true && enemies[i].CompareTag("Enemy"))
                    {
                        enemiesToShoot[enemiesCount] = enemies[i];
                        enemiesCount++;

                        if (enemiesCount >= maxEnemiesToShoot)
                        {
                            break;
                        }
                    }
                }
                countEnemy = enemiesCount;
                StartCoroutine(SpawnEnemiesCoroutine());
                step = stepMax;

            }
        }
    }
    private IEnumerator SpawnEnemiesCoroutine()
    {
        for (int i = 0; i < countEnemy; i++)
        {
            if (enemiesToShoot[i] != null)
            {
                if (enemiesToShoot[i].GetComponent<HealthPoint>())
                {
                    objHealth = enemiesToShoot[i].GetComponent<HealthPoint>();
                    objMove = enemiesToShoot[i].GetComponentInParent<Forward>();
                    Instantiate(lightObj, new Vector3(enemiesToShoot[i].transform.position.x,
                        enemiesToShoot[i].transform.position.y + 3), Quaternion.identity);

                    objHealth.healthPoint -= damage * ElectricityElement.Electricity;
                    objMove.isStunned = true;
                    objMove.stunnTime = stunTime;

                    if (objHealth.IsBobs == true)
                    {
                        enemiesToShoot[i].GetComponentInChildren<Animator>().SetBool("IsHit", true);
                    }
                    else
                    {
                        objHealth.ChangeToKick();
                    }
                }
                else if (enemiesToShoot[i].GetComponent<HealthBossPart>())
                {
                    objHealthB = enemiesToShoot[i].GetComponent<HealthBossPart>();
                    objMove = enemiesToShoot[i].transform.root.GetComponent<Forward>();
                    Instantiate(lightObj, new Vector3(enemiesToShoot[i].transform.position.x,
                        enemiesToShoot[i].transform.position.y + 3), Quaternion.identity);

                    objHealthB.healthPoint -= damage * ElectricityElement.Electricity;
                    objMove.isStunned = true;
                    objMove.stunnTime = stunTime / 2;

                    objHealthB.ChangeToKick();
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
