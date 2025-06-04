using FSMC.Runtime;
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
        AudioManager.instance.PlaySFX("WaterWave");
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
        objTransform.localScale = new Vector3(objTransform.localScale.x + Time.fixedDeltaTime * aceleration, 
            objTransform.localScale.y + Time.fixedDeltaTime * aceleration, 
            objTransform.localScale.z + Time.fixedDeltaTime * aceleration);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            FSMC_Executer objHealt = collision.GetComponent<FSMC_Executer>();
            ElementActiveDebuff debuff = collision.GetComponentInParent<ElementActiveDebuff>();
                debuff.ApplyEffect(Elements.status.Water, 5);
            objHealt.TakeDamage(damage * waterElement * debuff.elements.CurrentStatusValue(Elements.status.Water) / debuff.elements.CurrentStatusValue(Elements.status.Dirt));
            GameManager.Instance.FindStatName("towerWaveDamage", (damage * waterElement * debuff.elements.CurrentStatusValue(Elements.status.Water)) 
                / debuff.elements.CurrentStatusValue(Elements.status.Dirt));
        }
    }
}
