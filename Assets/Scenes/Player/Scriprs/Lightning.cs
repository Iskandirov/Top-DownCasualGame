using System.Collections;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public Collider2D[] enemies;
    public Collider2D[] enemiesToShoot;
    public GameObject lightObj;
    public float radius;
    HealthPoint objHealth;
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
                objHealth = enemiesToShoot[i].GetComponent<HealthPoint>();
                objMove = enemiesToShoot[i].GetComponentInParent<Forward>();
                Instantiate(lightObj, new Vector3(enemiesToShoot[i].transform.position.x,
                    enemiesToShoot[i].transform.position.y + 3), Quaternion.identity);

                objHealth.healthPoint -= damage * ElectricityElement.Electricity / objHealth.Electricity;
                objHealth.GetComponentInParent<ElementActiveDebuff>().SetBool("isElectricity", true);
                objHealth.GetComponentInParent<ElementActiveDebuff>().isElectricity = true;
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
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
