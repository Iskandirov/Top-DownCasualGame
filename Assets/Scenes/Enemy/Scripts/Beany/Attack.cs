using Unity.VisualScripting;
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
    public GameObject objectToHit;

    Forward objMove;
    Animator objAnim;
    PlayerManager player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objMove = GetComponent<Forward>();
        objAnim = GetComponent<Animator>();
        if (!objMove.isTutorial)
        {
            if (GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) > 0)
            {
                damageMax += GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) * 1.3f;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttack == true)
        {
            stepAttack -= Time.fixedDeltaTime;
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
        if (collision.collider.CompareTag("Shield") && isAttack == false)
        {
            objAnim.SetBool("IsHit", true);
            objectToHit = collision.gameObject;
            objMove.path.maxSpeed = 0;
            Shield shield = collision.collider.GetComponent<Shield>();
            shield.healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
            isAttack = true;
            if (shield.isThreeLevel)
            {
                GetComponentInChildren<HealthPoint>().healthPoint -= damage / 2;
                if (shield.isFiveLevel)
                {
                    shield.healthShieldMissed += damage;
                }
            }
        }
        else if (collision.collider.CompareTag("Player") && isAttack == false)
        {
            if (player.isInvincible == false)
            {
                if (!isRange)
                {
                    objMove.path.maxSpeed = 0;
                    objAnim.SetBool("IsHit", true);
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
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttack == false)
        {
            if (player.isInvincible == false)
            {
                if (!isRange)
                {
                    objectToHit = collision.gameObject;
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            objectToHit = null;
        }
    }
    public void EndAttack()
    {
        objMove.path.maxSpeed = GetComponent<Forward>().speedMax;
        objAnim.SetBool("IsHit", false);
    }
    public void DamageDeal()
    {
        if (objectToHit != null)
        {
            if (objectToHit.GetComponent<Animator>())
            {
                Shield shield = objectToHit.GetComponent<Shield>();
                HealthPoint EnemyHealth = objectToHit.GetComponent<HealthPoint>();
                    if (!player.isInvincible)
                    {
                        player.TakeDamage(damage);
                    }
                else if (shield != null)
                {
                    shield.healthShield -= damage;
                    GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
                }
                else if (EnemyHealth != null)
                {
                    EnemyHealth.healthPoint -= damage;
                    GameManager.Instance.FindStatName("bulletDamage", damage);
                }
               
            }
        }
    }
}
