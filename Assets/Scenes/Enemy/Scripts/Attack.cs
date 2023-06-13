using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage;
    public float stepAttack;
    public float stepAttackMax;
    public bool isAttack;
    public bool isRange;
    public bool isFive;
    public int chancToLure;
    public GameObject attackVFX;
    public GameObject attackVFXRange;
    public GameObject attackVFXRangePos;
    GameObject objVFX;
    public GameObject objectToHit;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttack == true)
        {
            stepAttack -= Time.deltaTime;
            if (stepAttack <= 0)
            {
                Destroy(objVFX);
                isAttack = false;
                stepAttack = stepAttackMax;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GetComponent<Forward>().isSummoned == false)
        {
            if (collision.CompareTag("Shield") && isAttack == false)
            {
                gameObject.GetComponent<Animator>().SetBool("IsAttack", true);
                gameObject.GetComponent<Forward>().speed = 0;
                objectToHit = collision.gameObject;
                collision.GetComponent<Shield>().healthShield -= damage;
                isAttack = true;
                if (collision.GetComponent<Shield>().isThreeLevel)
                {
                    gameObject.GetComponentInChildren<HealthPoint>().healthPoint -= damage / 2;
                    if (collision.GetComponent<Shield>().isFiveLevel)
                    {
                        collision.GetComponent<Shield>().healthShield‹Missed += damage;
                    }
                }
            }
            else if (collision.CompareTag("Player") && isAttack == false)
            {
                if (collision.GetComponent<Move>().isInvincible == false)
                {
                    if (!isRange)
                    {
                        gameObject.GetComponent<Forward>().speed = 0;
                        gameObject.GetComponent<Animator>().SetBool("IsAttack", true);
                        objectToHit = collision.gameObject;
                        Instantiate(attackVFX, gameObject.transform.position, Quaternion.identity);
                        isAttack = true;
                    }
                    else if (isRange)
                    {
                        Instantiate(attackVFXRange, attackVFXRangePos.transform.position, Quaternion.identity, attackVFXRangePos.transform);
                        isAttack = true;
                    }
                }
            }
        }
        else
        {
            if (collision.CompareTag("Enemy"))
            {
                chancToLure = Random.Range(0,10);
                gameObject.GetComponent<Forward>().speed = 0;
                if (collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || collision.GetComponent<HealthBossPart>())
                {
                    gameObject.GetComponent<Animator>().SetBool("IsAttack", true);
                    collision.GetComponent<HealthBossPart>().healthPoint -= damage;
                    collision.GetComponent<HealthBossPart>().ChangeToKick();
                }
                else if (!collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || !collision.GetComponent<HealthBossPart>())
                {
                    gameObject.GetComponent<Animator>().SetBool("IsAttack", true);
                    collision.GetComponent<HealthPoint>().healthPoint -= damage;
                    collision.GetComponent<HealthPoint>().ChangeToKick();
                }
                if (isFive && !collision.transform.root.GetComponent<Forward>().isSummoned && chancToLure > 7)
                {
                    collision.transform.root.GetComponent<Forward>().isSummoned = true;
                    collision.transform.root.GetComponent<Forward>().summonTime = 5;
                    // chance to lures enemies that bite
                }
            }
        }
    }
    public void EndAttack()
    {
        gameObject.GetComponent<Forward>().speed = gameObject.GetComponent<Forward>().speedMax;
        gameObject.GetComponent<Animator>().SetBool("IsAttack", false);
    }
    public void DamageDeal()
    {
        if (objectToHit.GetComponent<Animator>() && !GetComponent<Forward>().isSummoned)
        {
            objectToHit.GetComponent<Animator>().SetBool("IsHit", true);
            objectToHit.GetComponent<Health>().playerHealthPoint -= damage;
            objectToHit.GetComponent<Health>().playerHealthPointImg.fillAmount -= damage / objectToHit.GetComponent<Health>().playerHealthPointMax;
        }
    }
}
