using System;
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
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        ElectricityElement = transform.root.GetComponent<ElementsCoeficients>();
        step = gameObject.GetComponent<CDSkillObject>().CD;
        StartCoroutine(SetBumberToSkill());
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
        buttonActivateSkill = gameObject.GetComponent<CDSkillObject>().num + 1;
        keyCode = (KeyCode)((int)KeyCode.Alpha0 + buttonActivateSkill);
    }
    private void Update()
    {
        step -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        if (step <= 0 && Input.GetKeyDown(keyCode))
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
                if (enemies != null)
                {
                    StartCoroutine(SpawnEnemiesCoroutine());
                }
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

                
                if (objHealth.GetComponentInParent<ElementActiveDebuff>() != null && !objHealth.GetComponentInParent<ElementActiveDebuff>().IsActive("isElectricity", true))
                {
                    objHealth.GetComponentInParent<ElementActiveDebuff>().SetBool("isElectricity", true, true);
                    objHealth.GetComponentInParent<ElementActiveDebuff>().SetBool("isElectricity", true, false);
                }
                if (objMove != null)
                {
                    objMove.isStunned = true;
                    objMove.stunnTime = stunTime;
                }
                

                if (objHealth.IsBobs == true)
                {
                    enemiesToShoot[i].GetComponent<Animator>().SetBool("IsHit", true);
                }
                else
                {
                    objHealth.ChangeToKick();
                }
                objHealth.healthPoint -= damage * ElectricityElement.Electricity / objHealth.Electricity;
                FindObjectOfType<StatsCollector>().FindStatName("lightDamage", damage * ElectricityElement.Electricity / objHealth.Electricity);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
