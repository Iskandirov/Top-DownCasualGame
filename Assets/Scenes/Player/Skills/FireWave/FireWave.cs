using System.Collections;
using System.Numerics;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FireWave : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    public float burnDamage;
    public float fireElement;
    PlayerManager player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.transform.position;
        //transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * 20, transform.localScale.y + Time.deltaTime * 20, transform.localScale.z + Time.deltaTime * 20);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponentInParent<ElementActiveDebuff>() != null && !collision.GetComponentInParent<ElementActiveDebuff>().IsActive("isFire", true))
            {
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isFire", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isFire", true, false);
            }
            collision.GetComponent<HealthPoint>().TakeDamage((damage * fireElement * collision.GetComponent<HealthPoint>().Water) / collision.GetComponent<HealthPoint>().Fire);
            GameManager.Instance.FindStatName("fireWaveDamage", (damage * fireElement * collision.GetComponent<HealthPoint>().Water) / collision.GetComponent<HealthPoint>().Fire);
            if (burnDamage != 0 && collision != null)
            {
                collision.GetComponent<HealthPoint>().isBurn = true;
                collision.GetComponent<HealthPoint>().burnTime = 3;
                collision.GetComponent<HealthPoint>().burnDamage = burnDamage;
            }
        }
    }
}
