using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
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
    private void Update()
    {
        // ��� ��'���� � ��������� �������� � ������� ��������
        transform.position += mainDirection * baseMovementSpeed * Time.deltaTime;

        // ���� ���������� �������� � ���������
        chaoticChangeTimer -= Time.deltaTime;
        if (chaoticChangeTimer <= 0)
        {
            chaoticChangeTimer = chaoticChangeInterval;
            targetChaoticDirection = GetRandomDirection();
        }
        currentChaoticDirection = Vector3.MoveTowards(currentChaoticDirection, targetChaoticDirection, chaoticChangeSpeed * Time.deltaTime);

        // ��������� ��� �� ��������
        transform.position += currentChaoticDirection * chaoticMovementSpeed * Time.deltaTime;
    }

    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), 0f).normalized;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !collision.collider.isTrigger)
        {
            collision.collider.GetComponent<Health>().playerHealthPoint -= damage;
            collision.collider.GetComponent<Health>().playerHealthPointImg.fullFillImage.fillAmount -= damage / collision.collider.GetComponent<Health>().playerHealthPointMax;
            collision.collider.GetComponent<Animator>().SetBool("IsHit", true);
            Destroy(gameObject);
        }
    }
}