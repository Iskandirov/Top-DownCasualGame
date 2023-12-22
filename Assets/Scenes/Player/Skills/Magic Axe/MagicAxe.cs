using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MagicAxe : MonoBehaviour
{
    public float damage;
    public float rotationSpeed = 500f;
    public float speed = 15f;
    public float speedBack = 15f;
    Vector3 playerDirection;
    bool isBack;
    public bool isCreated;
    public bool isFirst;
    public bool isFive;
    Vector3 directionToCursor;
    public float timeToBack = 1f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = PlayerManager.instance.transform.position;
        // ���������� �������� �������
        directionToCursor = PlayerManager.instance.GetMousDirection();

        // ������ coroutine
        StartCoroutine(MoveToAndBack());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rotationSpeed += speed;
        // ��������� ���� ��������� ��'����
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotationSpeed);
        if (!isBack)
        {
            if (!isCreated)
            {
                // ��� ��'���� � �������� �������
                transform.position += new Vector3(directionToCursor.x * speed * 0.1f, directionToCursor.y * speed * 0.1f, 0f);
            }
            else
            {
                if (isFirst)
                {
                    // ��� ��'���� � �������� �������
                    transform.position -= Quaternion.AngleAxis(-80f, Vector3.down) * directionToCursor * speed * 0.03f;
                }
                else
                {
                    // ��� ��'���� � �������� �������
                    transform.position -= Quaternion.AngleAxis(-80f, Vector3.left) * directionToCursor * speed * 0.03f;
                }
            }
           
        }
        else
        {
            MoveBack();
        }
    }

    public IEnumerator MoveToAndBack()
    {
        yield return new WaitForSeconds(timeToBack);
        // ������ �������
        isBack = true;
    }

    private void MoveBack()
    {
        playerDirection = PlayerManager.instance.transform.position;

        // ��������� ������� ������� ��'����
        Vector3 currentPosition = transform.position;

        // ��������� �������� ���� ��'���� ������
        Vector3 forwardVelocity = currentPosition - playerDirection;


        // ��������� ��������� ��'���� �� ��������� �������
        float acceleration = speedBack * 2f;
        transform.position = Vector3.MoveTowards(currentPosition, playerDirection, 1.5f);

        // ���� ��'��� ���������� � ��������� �������, �� ����� ����
        if (forwardVelocity.magnitude < 5f)
        {
            if (isFive && !isCreated)
            {
                MagicAxe a = Instantiate(this, transform.position, Quaternion.identity);
                a.isCreated = true; 
                MagicAxe b = Instantiate(this, transform.position, Quaternion.identity);
                b.isCreated = true;
                b.isFirst = true;
            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            collision.GetComponent<HealthPoint>().TakeDamage(damage);
        }
    }
}
