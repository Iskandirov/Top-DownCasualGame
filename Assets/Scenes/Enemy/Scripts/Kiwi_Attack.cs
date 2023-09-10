using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiwi_Attack : MonoBehaviour
{
    public GameObject root; // ������ ��'����, ���� ���� �����������
    public float launchForce = 10.0f; // ���� �������
    public float delay;
    float delayMax;
    public void Start()
    {
        delayMax = delay;
    }
    public void Update()
    {
        delay -= Time.deltaTime;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ����������, �� ��'��� � �������-��� � �������
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (delay <= 0)
            {
                delay = delayMax;
                // �������� �������� �� ������
                Vector2 directionToPlayer = collision.transform.position - transform.position;

                // ��������� ����� ��'��� � ������������� �������
                GameObject newObject = Instantiate(root, transform.position, Quaternion.identity);

                // ��������� ����� ��'��� � �������� ������
                Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
                rb.velocity = directionToPlayer.normalized * launchForce;
            }
        }
    }
}
