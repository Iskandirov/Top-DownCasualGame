using System.Collections;
using UnityEngine;

public class BobmExplode : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    public float fire;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                EnemyController.instance.TakeDamage(collider.GetComponent<EnemyState>(), collider.GetComponent<EnemyState>().health - damage * fire);
                GameManager.Instance.FindStatName("bombDamage", damage * fire);
            }
            else if (collider.CompareTag("Barrel") && collider != null)
            {
                collider.gameObject.GetComponent<ObjectHealth>().health -= damage * fire;
            }
        }
        Destroy(gameObject);
    }
}
