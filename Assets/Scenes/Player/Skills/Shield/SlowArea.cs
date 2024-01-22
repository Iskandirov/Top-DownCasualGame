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
            if (collision.GetComponentInParent<Forward>() != null)
            {
                collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax * 0.5f / dirtElement;
            }
            if (!collision.GetComponentInParent<ElementActiveDebuff>().IsActive("isDirt", true))
            {
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isDirt", true, true);
                collision.GetComponentInParent<ElementActiveDebuff>().SetBool("isDirt", true, false);
            }
                
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision.GetComponentInParent<Forward>() != null)
        {
            collision.GetComponentInParent<Forward>().path.maxSpeed = collision.GetComponentInParent<Forward>().speedMax;
        }
    }
}
