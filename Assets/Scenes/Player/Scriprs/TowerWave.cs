using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWave : MonoBehaviour
{
    public float lifeTime;
    public float damage;
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
            if (collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || collision.GetComponent<HealthBossPart>())
            {
                collision.GetComponent<HealthBossPart>().healthPoint -= damage;
                collision.GetComponent<HealthBossPart>().ChangeToKick();
            }
            else if (!collision.gameObject.GetComponentInParent<ElementalBoss_Destroy>() || !collision.GetComponent<HealthBossPart>())
            {
                collision.GetComponent<HealthPoint>().healthPoint -= damage;
                collision.GetComponent<HealthPoint>().ChangeToKick();
            }
        }
    }
}
