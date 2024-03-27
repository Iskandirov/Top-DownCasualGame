using UnityEngine;

public class GetAllExp : MonoBehaviour
{
    float radius;
    CircleCollider2D colliderPlayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            colliderPlayer = PlayerManager.instance.GetComponent<CircleCollider2D>();
            radius = colliderPlayer.radius;
            colliderPlayer.radius *= radius;
            Invoke("DeactivateSucking", 1f);
            gameObject.SetActive(false);
        }
    }
    void DeactivateSucking()
    {
        colliderPlayer.radius = radius;
        Destroy(gameObject);
    }
}
