using System.Collections;
using UnityEngine;

public class MoveToPlayerStartPos : MonoBehaviour
{
    public Transform target; // ���������� �������� ��'���
    public float moveSpeed = 5.0f; // �������� ����
    public float damage = 10.0f; // ³������ ����

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private void Start()
    {
        target = FindObjectOfType<Move>().transform;
        rb = GetComponent<Rigidbody2D>();
        moveDirection = (transform.position - target.position).normalized;
        StartCoroutine(SelfDestroy());
    }
    public IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
    private void Update()
    {
        rb.velocity = -moveDirection * moveSpeed;
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
            Vector2 collisionNormal = collision.contacts[0].normal; // �������� ������� ��������
            Vector2 reflectedDirection = Vector2.Reflect(moveDirection, collisionNormal); // ���������� ������� ��������
            GetComponent<Rigidbody2D>().mass += 5500;
            moveDirection = reflectedDirection.normalized; // ������������ ����� �������� ����
        }
        if (collision.collider.GetComponent<MoveToPlayerStartPos>() != null) 
        {
            Vector2 collisionNormal = collision.contacts[0].normal; // �������� ������� ��������
            Vector2 reflectedDirection = Vector2.Reflect(moveDirection, collisionNormal); // ���������� ������� ��������
            GetComponent<Rigidbody2D>().mass += 5500;
            moveDirection = reflectedDirection.normalized; // ������������ ����� �������� ����
        }
    }
 
}
