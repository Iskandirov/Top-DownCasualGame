using Pathfinding;
using System.Collections;
using UnityEngine;

public class MoveToPlayerStartPos : MonoBehaviour
{
    public float moveSpeed = 5.0f; // �������� ����
    public float damage = 10.0f; // ³������ ����
    public Rigidbody2D objToFollow;
    //private Rigidbody2D rb;
    private Vector2 moveDirection;
    AIPath path;
    AIDestinationSetter destination;
    PlayerManager player;
    private void Start()
    {
        player = PlayerManager.instance;
        objToFollow.transform.position = player.transform.position;
        moveDirection = (transform.position - player.transform.position).normalized;

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
    private void FixedUpdate()
    {
        objToFollow.velocity = -moveDirection * moveSpeed;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !collision.collider.isTrigger)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Wall") && !collision.collider.isTrigger)
        {
            Vector2 collisionNormal = collision.contacts[0].normal; // �������� ������� ��������
            Vector2 reflectedDirection = Vector2.Reflect(moveDirection, collisionNormal); // ���������� ������� ��������
            //GetComponent<Rigidbody2D>().mass += 5500; // ���в��� ������ �²� ����� ��� ����� ������ ��� ²� �� ����Ѳ����� � �������� ������ � ����
            moveDirection = reflectedDirection.normalized; // ������������ ����� �������� ����
        }
    }
 
}
