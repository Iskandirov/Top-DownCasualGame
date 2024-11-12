using System.Collections;
using UnityEngine;

public class Tornadic_Move : MonoBehaviour
{
    public float damage = 20f; 
    public float baseMovementSpeed = 2.0f; // ������ �������� ���� � ��������� ��������
    public float chaoticChangeInterval = 2.0f; // �������� ��� ���� ���������� ��������
    public float chaoticChangeSpeed = 1.0f; // �������� ���� ���������� ��������
    public float chaoticMovementSpeed = 10.0f; // �������� ���� �� �������� �����

    public Vector3 mainDirection;
    private Vector3 currentChaoticDirection;
    private Vector3 targetChaoticDirection;
    private float chaoticChangeTimer;
    Transform objTransform;

    private void Start()
    {
        objTransform = transform;
        //mainDirection = new Vector3(1f, 0f, 0f); // ������� �������� ��������
        currentChaoticDirection = Vector3.zero;
        targetChaoticDirection = GetRandomDirection();
        chaoticChangeTimer = chaoticChangeInterval;
        Invoke("SelfDestroy",10f);
    }
    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        // ��� ��'���� � ��������� �������� � ������� ��������
        objTransform.position += mainDirection * baseMovementSpeed * Time.fixedDeltaTime;

        // ���� ���������� �������� � ��������
        chaoticChangeTimer -= Time.fixedDeltaTime;
        if (chaoticChangeTimer <= 0)
        {
            chaoticChangeTimer = chaoticChangeInterval;
            targetChaoticDirection = GetRandomDirection();
        }
        currentChaoticDirection = Vector3.MoveTowards(currentChaoticDirection, targetChaoticDirection, chaoticChangeSpeed * Time.fixedDeltaTime);

        // ��������� ��� �� ��������
        objTransform.position += currentChaoticDirection * chaoticMovementSpeed * Time.fixedDeltaTime;
    }
    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), 0f).normalized;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !collision.collider.isTrigger && !PlayerManager.instance.isInvincible)
        {
            collision.collider.GetComponent<PlayerManager>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Shield"))
        {
            collision.collider.GetComponent<Shield>().healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
            Destroy(gameObject);
        }
    }
}
