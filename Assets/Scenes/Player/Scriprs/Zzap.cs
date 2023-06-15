using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zzap : MonoBehaviour
{
    public GameObject copie;
    public float damage;
    public float x;
    public float y;
    public float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
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
        transform.position = new Vector2(copie.transform.position.x + x, copie.transform.position.y + y);// +45
    }
    private void OnTriggerEnter2D(Collider2D collision)
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
