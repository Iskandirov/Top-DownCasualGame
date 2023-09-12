using System.Collections;
using UnityEngine;

public class TowerWave : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    public float waterElement;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.transform.position;
        StartCoroutine(TimerSpell());
    }
    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * 20, transform.localScale.y + Time.deltaTime * 20, transform.localScale.z + Time.deltaTime * 20);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HealthPoint objHealt = collision.GetComponent<HealthPoint>();
            if (collision.GetComponentInParent<ElementActiveDebuff>() != null && !collision.GetComponentInParent<ElementActiveDebuff>().IsActive("isWater", true))
            {
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isWater", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isWater", true, false);
            }
            objHealt.healthPoint -= (damage * waterElement * objHealt.Water) / objHealt.Dirt;
            objHealt.ChangeToKick();
        }
    }
}
