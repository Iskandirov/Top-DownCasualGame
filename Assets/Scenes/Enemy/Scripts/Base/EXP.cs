using UnityEngine;

public class EXP : MonoBehaviour
{
    public float expBuff;

    public bool itWasInPlayerZone; // Значення прискорення
    public float speed;
    public float acceleration = 10f; // Значення прискорення

    PlayerManager player;
    Transform objTransform;
    public void Awake()
    {
        player = PlayerManager.instance;
        objTransform = transform;
    }
    private void FixedUpdate()
    {
        if (itWasInPlayerZone)
        {
            speed += acceleration * Time.fixedDeltaTime;
            objTransform.position = Vector2.MoveTowards(objTransform.position, player.objTransform.position, speed * Time.fixedDeltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            GameManager.Instance.expiriencepoint.fillAmount += expBuff * player.multiply / player.expNeedToNewLevel;
            Destroy(gameObject);
        }
    }
}
