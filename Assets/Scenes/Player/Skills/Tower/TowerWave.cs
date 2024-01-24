using System.Collections;
using UnityEngine;

public class TowerWave : MonoBehaviour
{
    public float lifeTime;
    public float damage;
    public float waterElement;
    public float aceleration = 20;
    Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        objTransform = transform;
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
        objTransform.localScale = new Vector3(objTransform.localScale.x + Time.fixedDeltaTime * aceleration, objTransform.localScale.y + Time.fixedDeltaTime * aceleration, objTransform.localScale.z + Time.fixedDeltaTime * aceleration);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HealthPoint objHealt = collision.GetComponent<HealthPoint>();
            ElementActiveDebuff element = collision.GetComponentInParent<ElementActiveDebuff>();
            if (element != null && !element.IsActive("isWater", true))
            {
                element.SetBool("isWater", true, true);
                element.SetBool("isWater", true, false);
            }
            objHealt.healthPoint -= (damage * waterElement * objHealt.Water) / objHealt.Dirt;
            GameManager.Instance.FindStatName("towerWaveDamage", (damage * waterElement * objHealt.Water) / objHealt.Dirt);
        }
    }
}
