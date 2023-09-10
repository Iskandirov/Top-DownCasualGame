using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Bullet bullet;
    public GameObject ShootPoint;
    public float stepShoot;
    public float damageToGive;
    public float attackSpeed;
    public float launchForce = 10f;  // ���� ������� ��'����

    //public float movementSpeed = 5f;  // �������� ���� ��'����
    public float circleRadius = 5f;  // ����� ����

    public GameObject shoot;
    public bool isLevelTwo;
    public bool isLevelFive;
    public int secondBulletCount;
    // Start is called before the first frame update
    void Start()
    {
        bullet.GetComponent<Bullet>().damage = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ShootBullet(ShootPoint);
    }
   
    public void ShootBullet(GameObject ShootPointObj)
    {
        stepShoot += Time.deltaTime;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (stepShoot >= attackSpeed)
        {
            if (Input.GetMouseButton(0))  // ����������, �� ��������� ��� ������ ����
            {
                stepShoot = 0;
                shoot = ShootPointObj;
                // �������� ������� ������� � ������� �����������
                //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;  // ���������� ���������� Z �� ������

                // ��������� ��'��� � ������������� �������
                Bullet newObject = Instantiate(bullet, transform.position, Quaternion.identity);
                newObject.damage = damageToGive;
                newObject.isPiers = isLevelFive;
                if (isLevelTwo)
                {
                    for (int i = 1; i < secondBulletCount + 1; i++)
                    {
                        Invoke("CreateBullet", i * 0.3f);
                    }
                }
                // ��������� �������� �� �������
                Vector2 directionBullet = mousePosition - transform.position;

                // ��������� ��'��� � �������� ��������
                Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
                rb.AddForce(directionBullet.normalized * launchForce, ForceMode2D.Impulse);
                float angleShot = Mathf.Atan2(directionBullet.y, directionBullet.x) * Mathf.Rad2Deg;
                newObject.transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
            }
        }
        mousePosition.z = 0f;  // ���������� ���������� Z �� ������

        // ��������� ������� �� ������ �� ������� �������
        Vector3 direction = mousePosition - transform.position;
        direction = direction.normalized * circleRadius;

        // ��������� ������ ������� ��'���� �� ���
        Vector3 targetPosition = transform.position + direction;

        // ������ ��'��� �� ������ �������
        ShootPoint.transform.position = targetPosition;

        // ��������� ���� � �������� �������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ShootPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public void CreateBullet()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bullet newObject = Instantiate(bullet, transform.position, Quaternion.identity);
        newObject.damage = damageToGive;
        newObject.isPiers = isLevelFive;

        Vector2 directionBullet = mousePosition - transform.position;

        // ��������� ��'��� � �������� ��������
        Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
        rb.AddForce(directionBullet.normalized * launchForce, ForceMode2D.Impulse);
        float angleShot = Mathf.Atan2(directionBullet.y, directionBullet.x) * Mathf.Rad2Deg;
        newObject.transform.rotation = Quaternion.AngleAxis(angleShot + 90, Vector3.forward);
    }
}
