using UnityEngine;

public class Attack : MonoBehaviour
{
    //public float damage;
    //public float damageMax;
    //public float stepAttack;
    //public float stepAttackMax;
    //public bool isAttack;
    //public bool isRange;
    //public bool isFive;
    //public int chancToLure;
    //public GameObject attackVFX;
    //public GameObject attackVFXRange;
    //public GameObject attackVFXRangePos;
    ////GameObject objVFX;
    //public GameObject objectToHit;

    //public Forward objMove;
    //public Animator objAnim;
    //PlayerManager player;
    //GameManager manager;
    // Start is called before the first frame update
    //void Start()
    //{
    //    player = PlayerManager.instance;
    //    manager = GameManager.Instance;
    //    //if (!objMove.isTutorial)
    //    //{
    //    //    if (manager.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) > 0)
    //    //    {
    //    //        damageMax += manager.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) * 1.3f;
    //    //    }
    //    //}
    //}

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    if (isAttack == true)
    //    {
    //        stepAttack -= Time.fixedDeltaTime;
    //        if (stepAttack <= 0)
    //        {
    //            Destroy(objVFX);
    //            isAttack = false;
    //            stepAttack = stepAttackMax;
    //        }
    //    }
    //}
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Shield") && isAttack == false)
    //    {
    //        objAnim.SetBool("IsHit", true);
    //        objectToHit = collision.gameObject;
    //        //objMove.path.maxSpeed = 0;
    //        Shield shield = collision.collider.GetComponent<Shield>();
    //        shield.healthShield -= damage;
    //        manager.FindStatName("ShieldAbsorbedDamage", damage);
    //        isAttack = true;
    //        if (shield.basa.stats[2].isTrigger)
    //        {
    //            GetComponent<EnemyState>().HealthDamage(GetComponent<EnemyState>().health - damage / 2);
    //            if (shield.basa.stats[4].isTrigger)
    //            {
    //                shield.healthShieldMissed += damage;
    //            }
    //        }
    //    }
    //    else if (collision.collider.CompareTag("Player") && isAttack == false)
    //    {
    //            if (!isRange)
    //            {
    //                //objMove.path.maxSpeed = 0;
    //                objAnim.SetBool("IsHit", true);
    //                objectToHit = collision.gameObject;
    //                Instantiate(attackVFX, transform.position, Quaternion.identity);
    //                isAttack = true;
    //            }
    //            else if (isRange)
    //            {
    //                Instantiate(attackVFXRange, attackVFXRangePos.transform.position, Quaternion.identity, attackVFXRangePos.transform);
    //                isAttack = true;
    //            }
    //    }
    //}
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player") && isAttack == false)
    //    {
    //            if (!isRange)
    //            {
    //                objectToHit = collision.gameObject;
    //            }
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        objectToHit = null;
    //    }
    //}
    //public void EndAttack()
    //{
    //    //objMove.path.maxSpeed = GetComponent<Forward>().speedMax;
    //    objAnim.SetBool("IsHit", false);
    //}
    //public void DamageDeal()
    //{
    //    if (objectToHit != null)
    //    {
    //        if (objectToHit.GetComponent<Animator>())
    //        {
    //            Shield shield = objectToHit.GetComponent<Shield>();
    //            EnemyState EnemyHealth = objectToHit.GetComponent<EnemyState>();
    //            if (shield != null)
    //            {
    //                shield.healthShield -= damage;
    //                manager.FindStatName("ShieldAbsorbedDamage", damage);
    //            }
    //            else if (EnemyHealth != null)
    //            {
    //                EnemyHealth.HealthDamage(EnemyHealth.health - damage);
    //                manager.FindStatName("bulletDamage", damage);
    //            }
    //            else
    //            {
    //                player.TakeDamage(damage);
    //            }

    //        }
    //    }
    //}
}
