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
                if (collider.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || collider.GetComponent<HealthBossPart>())
                {
                    collider.GetComponent<HealthBossPart>().healthPoint -= damage * fire;
                    collider.GetComponent<HealthBossPart>().ChangeToKick();

                }
                else if (!collider.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || !collider.GetComponent<HealthBossPart>())
                {
                    collider.GetComponent<HealthPoint>().healthPoint -= damage * fire;
                    collider.GetComponent<HealthPoint>().ChangeToKick();
                }
            }
            else if (collider.CompareTag("Barrel"))
            {
                collider.gameObject.GetComponent<ObjectHealth>().health -= damage * fire;
            }
        }
        Destroy(gameObject);
    }
}
