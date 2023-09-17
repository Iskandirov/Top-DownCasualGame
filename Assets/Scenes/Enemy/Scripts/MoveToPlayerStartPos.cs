using Pathfinding;
using System.Collections;
using UnityEngine;

public class MoveToPlayerStartPos : MonoBehaviour
{
    public Transform target; // Початковий цільовий об'єкт
    public float moveSpeed = 5.0f; // Швидкість руху
    public float damage = 10.0f; // Відстань руху
    public Rigidbody2D objToFollow;
    //private Rigidbody2D rb;
    private Vector2 moveDirection;
    AIPath path;
    AIDestinationSetter destination;
    private void Start()
    {
        target = FindObjectOfType<Move>().transform;
        //rb = GetComponent<Rigidbody2D>();
        objToFollow.transform.position = target.transform.position;
        moveDirection = (transform.position - target.position).normalized;

        path = GetComponent<AIPath>();
        path.maxSpeed = moveSpeed;
        destination = GetComponent<AIDestinationSetter>();
        destination.target = objToFollow.transform;

        StartCoroutine(SelfDestroy());
    }
    public IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(5);
        Destroy(objToFollow);
        Destroy(gameObject);
    }
    private void Update()
    {
        objToFollow.velocity = -moveDirection * moveSpeed;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !collision.collider.isTrigger)
        {
            collision.collider.GetComponent<Health>().GetComponent<Animator>().SetBool("IsHit", true);
            collision.collider.GetComponent<Health>().playerHealthPoint -= damage;
            collision.collider.GetComponent<Health>().playerHealthPointImg.fullFillImage.fillAmount -= damage / collision.collider.GetComponent<Health>().playerHealthPointMax;
            Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Wall") && !collision.collider.isTrigger)
        {
            Vector2 collisionNormal = collision.contacts[0].normal; // Отримуємо нормаль зіткнення
            Vector2 reflectedDirection = Vector2.Reflect(moveDirection, collisionNormal); // Обчислюємо відбитий напрямок
            GetComponent<Rigidbody2D>().mass += 5500;
            moveDirection = reflectedDirection.normalized; // Встановлюємо новий напрямок руху
        }
        if (collision.collider.GetComponent<MoveToPlayerStartPos>() != null) 
        {
            Vector2 collisionNormal = collision.contacts[0].normal; // Отримуємо нормаль зіткнення
            Vector2 reflectedDirection = Vector2.Reflect(moveDirection, collisionNormal); // Обчислюємо відбитий напрямок
            GetComponent<Rigidbody2D>().mass += 5500;
            moveDirection = reflectedDirection.normalized; // Встановлюємо новий напрямок руху
        }
    }
 
}
