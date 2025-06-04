using FSMC.Runtime;
using UnityEngine;

public class SlowArea : MonoBehaviour
{
    public float dirtElement;
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameManager.Instance.FindStatName("slowedTime", Time.fixedDeltaTime);
            collision.GetComponent<FSMC_Executer>().SetFloat("SlowPercent", 0.5f / dirtElement);
            collision.GetComponent<FSMC_Executer>().SetFloat("SlowTime", .5f);
            collision.GetComponent<FSMC_Executer>().StateMachine.SetCurrentState("Slow", collision.GetComponent<FSMC_Executer>());
            ElementActiveDebuff debuff = collision.GetComponentInParent<ElementActiveDebuff>();
            if (debuff != null)
            {
                debuff.ApplyEffect(Elements.status.Dirt, 5);
            }
        }
    }
    //
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy") && collision.GetComponentInParent<Forward>() != null)
    //    {
    //        collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax;
    //    }
    //}
}
