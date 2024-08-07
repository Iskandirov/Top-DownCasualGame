using Pathfinding;
using System.Collections;
using UnityEngine;

public class MoveToPlayerStartPos : MonoBehaviour
{
    public float moveSpeed = 5.0f; // �������� ����
    public float damage = 10.0f; // ³������ ����
    public Rigidbody2D objToFollow;
    public Rigidbody2D rb;
    //private Rigidbody2D rb;
    private Vector2 moveDirection;
    AIPath path;
    AIDestinationSetter destination;
    PlayerManager player;
    public Transform objTransform;
    private void Start()
    {
        moveSpeed = GetComponent<AIPath>().maxSpeed;
        GetComponent<AIPath>().enabled= false;
        player = PlayerManager.instance;
        objToFollow.transform.position = player.transform.position;
        moveDirection = (transform.position - player.transform.position).normalized;

        //path = GetComponent<AIPath>();
        //path.maxSpeed = moveSpeed;
        //destination = GetComponent<AIDestinationSetter>();
        //destination.target = objToFollow.transform;

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
        //objToFollow.velocity = -moveDirection * moveSpeed;
        rb.velocity = new Vector2(objTransform.position.x + moveSpeed * Time.fixedDeltaTime, objTransform.position.y + moveSpeed * Time.fixedDeltaTime);
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
            moveDirection = reflectedDirection.normalized; // ������������ ����� �������� ����
        }
    }
 
}
