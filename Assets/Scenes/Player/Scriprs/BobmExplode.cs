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
                    collider.GetComponent<HealthPoint>().healthPoint -= damage * fire;
                    collider.GetComponent<HealthPoint>().ChangeToKick();
            }
            else if (collider.CompareTag("Barrel") && collider != null)
            {
                collider.gameObject.GetComponent<ObjectHealth>().health -= damage * fire;
            }
        }
        Destroy(gameObject);
    }
}
