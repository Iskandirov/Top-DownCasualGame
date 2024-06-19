using Pathfinding;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    [field: SerializeField] public bool isSlowed { get; private set; }
    [field: SerializeField] public bool isStun { get; private set; }
    public bool isFreezed;
    [field: SerializeField] public bool isBurn { get; private set; }
    [field: SerializeField] public bool isAttack { get; private set; }
    [field: SerializeField] public string mobName { get; private set; }
    [field: SerializeField] public string type { get; private set; }
    [field: SerializeField] public float attackSpeed { get; set; }
    [field: SerializeField] public float health { get; private set; }
    [field: SerializeField] public float damage { get; private set; }
    [field: SerializeField] public int timesPerOne { get; private set; }
    [field: SerializeField] public Transform objTransform { get; private set; }
    [field: SerializeField] public Transform AttackObj { get; private set; }
    [field: SerializeField] public GameObject ElementsParent { get; private set; }
    [SerializeField] public GameObject RepositionPoint;
    [SerializeField] public AIDestinationSetter destination;
    [SerializeField] public CircleCollider2D colider;
    [SerializeField] public AIPath path;

    public GameObject objectToHit;
    public void SetStunned()
    {
        isSlowed = true;
        isStun = true;
    }
    public void SetNotStunned()
    {
        isSlowed = false;
        isStun = false;
    }
    public void Stun(float stunTime)
    {
        GetComponent<Animator>().SetBool("IsStuned", true);
        Invoke("NotStun", stunTime);
    }
    public void NotStun()
    {
        GetComponent<Animator>().SetBool("IsStuned", false);
    }

    public void SetType(string value)
    {
        type = value;
    }

    public void SetBurn()
    {
        isBurn = !isBurn;
    }
    public void SetIsNotAttack()
    {
        GetComponent<Animator>().SetBool("Attack", false);
        isAttack = false;
    }
    public void Attack()
    {
        isAttack = true;
        //Instantiate(AttackObj, objTransform.position, Quaternion.identity);
    }
    public void SetAttackSpeed(float newAttackSpeed)
    {
        attackSpeed = newAttackSpeed;
    }
    public void HealthDamage(float newHealth)
    {
        GetComponent<Animator>().SetTrigger("Hit");
        health = newHealth;
    }
    public void StopHit()
    {
        GetComponent<Animator>().SetTrigger("Hit");
    }
    public void Damage(float damage)
    {
        this.damage = damage;
    }

    //Attack 
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.CompareTag("Shield") || collision.CompareTag("Player")) && !collision.isTrigger && attackSpeed <= 0)
        {
            objectToHit = collision.gameObject;
            GetComponent<Animator>().SetBool("Attack", true);
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