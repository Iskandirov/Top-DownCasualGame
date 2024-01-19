using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : SkillBaseMono
{
    public Collider2D[] enemies;
    public Collider2D[] enemiesToShoot;
    public GameObject lightObj;
    HealthPoint objHealth;
    Forward objMove;
    public int maxEnemiesToShoot;
    public float stunTime;
    public float step;
    public float spawnInterval;
    int countEnemy;
    PlayerManager player;
    

    //int buttonActivateSkill;
    //KeyCode keyCode;
    //public AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
       
        player = PlayerManager.instance;
        //basa = SetToSkillID(gameObject);
        if (basa.stats[0].isTrigger)
        {
            basa.damage = basa.stats[0].value;
        }
        if (basa.stats[1].isTrigger)
        {
            maxEnemiesToShoot += (int)basa.stats[1].value;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.damage *= basa.stats[2].value;
        }
        if (basa.stats[3].isTrigger)
        {
            stunTime = basa.stats[3].value;
        }
        if (basa.stats[4].isTrigger)
        {
            basa.skill.skill.stepMax -= basa.stats[4].value;
        }
        //step = basa.stepMax;
        //step = gameObject.GetComponent<CDSkillObject>().CD;
        //StartCoroutine(SetBumberToSkill());
    }
    //private IEnumerator SetBumberToSkill()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    //buttonActivateSkill = gameObject.GetComponent<CDSkillObject>().num + 1;
    //    keyCode = (KeyCode)((int)KeyCode.Alpha0 + buttonActivateSkill);
    //}
    //void FixedUpdate()
    //{
    //    step -= Time.fixedDeltaTime;
    //}
    private void Update()
    {
        //if (step <= 0 && Input.GetKeyDown(keyCode))
        //{
            enemiesToShoot = new Collider2D[maxEnemiesToShoot];
            enemies = Physics2D.OverlapCircleAll(gameObject.transform.position, basa.radius);

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
                //step = stepMax;
            }
        //}
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
                //sound.Play();
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
                    enemiesToShoot[i].GetComponentInParent<Animator>().SetBool("IsHit", true);
                }
                else
                {

                }
                objHealth.healthPoint -= basa.damage * player.Electricity / objHealth.Electricity;
                GameManager.Instance.FindStatName("lightDamage", basa.damage * player.Electricity / objHealth.Electricity);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
