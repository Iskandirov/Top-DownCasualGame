using FSMC.Runtime;
using System.Collections;
using UnityEngine;

public class BobmExplode : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    public float fire;
    public GameObject explode;
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = PlayerManager.instance.objTransform.position;
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 20f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.GetComponent<FSMC_Executer>().TakeDamage(damage * fire);
                GameManager.Instance.FindStatName("bombDamage", damage * fire);
            }
            else if (collider.CompareTag("Barrel") && collider != null)
            {
                collider.gameObject.GetComponent<ObjectHealth>().TakeDamage();
            }
        }
        Instantiate(explode, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
