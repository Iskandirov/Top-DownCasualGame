using UnityEngine;
using UnityEngine.SceneManagement;

public class Attack : MonoBehaviour
{
    public float damage;
    public float damageMax;
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
        if (FindObjectOfType<KillCount>().LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) > 0)
        {
            damageMax *= FindObjectOfType<KillCount>().LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) * 0.5f;
        }
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
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (objMove.isSummoned == false)
        {
            if (collision.collider.CompareTag("Shield") && isAttack == false)
            {
                objAnim.SetBool("IsHit", true);
                objMove.speed = 0;
                collision.collider.GetComponent<Shield>().healthShield -= damage;
                isAttack = true;
                if (collision.collider.GetComponent<Shield>().isThreeLevel)
                {
                    gameObject.GetComponentInChildren<HealthPoint>().healthPoint -= damage / 2;
                    if (collision.collider.GetComponent<Shield>().isFiveLevel)
                    {
                        collision.collider.GetComponent<Shield>().healthShieldMissed += damage;
                    }
                }
            }
            else if (collision.collider.CompareTag("Player") && isAttack == false)
            {
                if (collision.collider.GetComponent<Move>().isInvincible == false)
                {
                    if (!isRange)
                    {
                        objMove.speed = 0;
                        objAnim.SetBool("IsHit", true);
                        objectToHit = collision.collider.GetComponent<Health>();
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
            if (collision.collider.CompareTag("Enemy") && isAttack == false)
            {
                chancToLure = Random.Range(0, 10);
                objMove.speed = 0;
                objAnim.SetBool("IsHit", true);
                collision.collider.GetComponent<HealthPoint>().healthPoint -= damage;
                collision.collider.GetComponent<HealthPoint>().ChangeToKick();
                if (isFive && !collision.transform.root.GetComponent<Forward>().isSummoned && chancToLure > 7)
                {
                    collision.transform.root.GetComponent<Forward>().isSummoned = true;
                    collision.transform.root.GetComponent<Forward>().summonTime = 5;
                    // chance to lures enemies that bite
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
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
            }
        }
    }
}
