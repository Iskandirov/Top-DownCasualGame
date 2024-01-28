using UnityEngine;

public class EnemyState : MonoBehaviour
{
    [field: SerializeField] public bool isSlowed { get; private set; }
    [field: SerializeField] public bool isBurn { get; private set; }
    [field: SerializeField] public bool isAttack { get; private set; }
    [field: SerializeField] public string mobName { get; private set; }
    [field: SerializeField] public float attackSpeed { get; set; }
    [field: SerializeField] public float health { get; private set; }
    [field: SerializeField] public float damage { get; private set; }
    [field: SerializeField] public int timesPerOne { get; private set; }
    [field: SerializeField] public Transform objTransform { get; private set; }
    public GameObject objectToHit;
    public void SetStunned()
    {
        isSlowed = true;
    } 
    public void SetNotStunned()
    {
        isSlowed = false;
    }
    public void SetBurn()
    {
        isBurn = !isBurn;
    } 
    public void SetIsAttack()
    {
        isAttack = true;
        //GetComponent<Animator>().SetBool("IsHit", isAttack);
    } 
    public void SetIsNotAttack()
    {
        isAttack = false;
        GetComponent<Animator>().SetBool("IsHit", isAttack);
    } 
    public void SetAttackSpeed(float newAttackSpeed)
    {
        attackSpeed = newAttackSpeed;
    }
    public void HealthDamage(float newHealth)
    {
        health = newHealth;
    } 
    public void Damage(float damage)
    {
        this.damage = damage;
    }

    //Attack 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.collider.CompareTag("Shield") || collision.collider.CompareTag("Player")) && !collision.collider.isTrigger)
        {
            objectToHit = collision.gameObject;
            GetComponent<Animator>().SetBool("IsHit", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            objectToHit = null;
        }
    }
}
