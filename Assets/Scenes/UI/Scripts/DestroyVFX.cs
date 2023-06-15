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
