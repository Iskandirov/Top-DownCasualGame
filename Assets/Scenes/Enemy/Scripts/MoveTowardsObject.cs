using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsObject : MonoBehaviour
{
    public Transform playerTransform;
    public float maxSpeed = 5f; // ����������� �������� ��'����
    public float acceleration = 0.1f; // ��������� ��������
    public float angle;

    private Vector2 velocity;

    Rigidbody2D rb;
    private void Start()
    {
        // ������������ ������ �������� � ��������� �������� (�������� ������)
        velocity = Vector2.zero;
        gameObject.AddComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.angularDrag = 0;
        rb.mass = 100;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ��������� �������� �� ������
        Vector2 direction = playerTransform.position - transform.position;
        direction.Normalize();

        // ��������� ��� �� ��������� �� ������ � �������� ��������� ���� ��'����
        angle = Vector2.Angle(velocity, direction);

        // ���� ��� �� ��������� ������ �� 100 ������� - �������� ��������
        if (angle > 20f)
        {
            // �������� �������� � ���������� ������������
            velocity = Vector2.Lerp(velocity, direction * maxSpeed, Time.deltaTime * acceleration);
        }
        else // ���� ��� ������ - �������� ��������
        {
            // �������� �������� � ���������� ������������
            velocity = Vector2.Lerp(velocity, direction * maxSpeed, Time.deltaTime * acceleration);
        }

        // �������� ����������� ��������
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        // ������ ��'��� �� �������, �� ������� ��������, ��������� �� ��� ���������
        transform.Translate(velocity * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �������� ������ ������ ����������
        Vector2 normal = collision.contacts[0].normal;

        // ����������� ������� �������� ������� ������ ��������
        velocity = Vector2.Reflect(velocity, normal) * 0.8f;

        // ��������� ���� � ��������� �� ������� ������
        rb.AddForce(normal * 5f, ForceMode2D.Impulse);
    }
}