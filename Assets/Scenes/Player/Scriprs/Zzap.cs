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
    public float damageTick;
    public float damageTickMax;
    public float electicElement;
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
        damageTick -= Time.deltaTime;
        transform.position = new Vector2(copie.transform.position.x + x, copie.transform.position.y + y);// +45
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageTick <= 0)
        {
            if (collision.CompareTag("Enemy"))
            {
                    collision.GetComponent<HealthPoint>().healthPoint -= damage * electicElement;
                    collision.GetComponent<HealthPoint>().ChangeToKick();
            }
            damageTick = damageTickMax;
        }
       
    }
}
