using UnityEngine;
using UnityEngine.UI;

public class EXP : MonoBehaviour
{
    public float expBuff;

    public bool itWasInPlayerZone; // �������� �����������
    public float speed;
    public float acceleration = 10f; // �������� �����������

    public PlayerManager player;
    public void Awake()
    {
        player = PlayerManager.instance;
    }
    private void FixedUpdate()
    {
        if (itWasInPlayerZone)
        {
            speed += acceleration * Time.fixedDeltaTime; // �������� �������� � ������ ������
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.fixedDeltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            player.expiriencepoint.fillAmount += expBuff * player.multiply / player.expNeedToNewLevel;
            Destroy(gameObject);
        }
    }
}
