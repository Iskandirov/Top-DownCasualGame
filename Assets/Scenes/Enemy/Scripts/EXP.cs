using UnityEngine;
using UnityEngine.UI;

public class EXP : MonoBehaviour
{
    public float expBuff;

    public bool itWasInPlayerZone; // �������� �����������
    public float speed;
    public float acceleration = 10f; // �������� �����������

    public Expirience playerExp;
    public void Start()
    {
        playerExp = FindObjectOfType<Expirience>();
    }
    private void FixedUpdate()
    {
        if (itWasInPlayerZone)
        {
            speed += acceleration * Time.deltaTime; // �������� �������� � ������ ������
            transform.position = Vector2.MoveTowards(transform.position, playerExp.transform.position, speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            playerExp.expiriencepoint.fillAmount += expBuff / playerExp.expNeedToNewLevel;
            Destroy(gameObject);
        }
    }
}
