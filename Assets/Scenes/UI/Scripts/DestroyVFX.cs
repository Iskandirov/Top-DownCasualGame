using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DestroyVFX : MonoBehaviour
{
    public float delay;
    public bool isGoingAttack;
    public GameObject Bullet;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("CreateObject", delay);// ��������� ������ �� ��������� ��'����
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void CreateObject()
    {
        if (isGoingAttack)
        {
            // ��������� ����� ��'��� � ���� �������
            Instantiate(Bullet, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
