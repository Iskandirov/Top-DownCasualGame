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
    Health objectToHit;

    Forward objMove;
    Animator objAnim;
    // Start is called before the first frame update
    void Start()
    {
        objMove = GetComponent<Forward>();
        objAnim = GetComponent<Animator>();
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
        if (objMove.isSummoned == false)
        {
            if (collision.CompareTag("Shield") && isAttack == false)
            {
                objAnim.SetBool("IsHit", true);
                objMove.speed = 0;
                collision.GetComponent<Shield>().healthShield -= damage;
                isAttack = true;
                if (collision.GetComponent<Shield>().isThreeLevel)
                {
                    gameObject.GetComponentInChildren<HealthPoint>().healthPoint -= damage / 2;
                    if (collision.GetComponent<Shield>().isFiveLevel)
                    {
                        collision.GetComponent<Shield>().healthShieldMissed += damage;
                    }
                }
            }
            else if (collision.CompareTag("Player") && isAttack == false)
            {
                if (collision.GetComponent<Move>().isInvincible == false)
                {
                    if (!isRange)
                    {
                        objMove.speed = 0;
                        objAnim.SetBool("IsHit", true);
                        objectToHit = collision.GetComponent<Health>();
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
                chancToLure = Random.Range(0, 10);
                objMove.speed = 0;
                if (collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || collision.GetComponent<HealthBossPart>())
                {
                    objAnim.SetBool("IsHit", true);
                    collision.GetComponent<HealthBossPart>().healthPoint -= damage;
                    collision.GetComponent<HealthBossPart>().ChangeToKick();
                }
                else if (!collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || !collision.GetComponent<HealthBossPart>())
                {
                    objAnim.SetBool("IsHit", true);
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
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            objectToHit = null;
        }
    }
    public void EndAttack()
    {
        objMove.speed = GetComponent<Forward>().speedMax;
        objAnim.SetBool("IsHit", false);
    }
    public void DamageDeal()
    {
        if (objectToHit != null)
        {
            if (objectToHit.GetComponent<Animator>() && !objMove.isSummoned)
            {
                objectToHit.GetComponent<Animator>().SetBool("IsHit", true);
                objectToHit.playerHealthPoint -= damage;
                objectToHit.playerHealthPointImg.fillAmount -= damage / objectToHit.playerHealthPointMax;
                objectToHit.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }
}
