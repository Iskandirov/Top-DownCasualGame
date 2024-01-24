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
            Forward move = collision.GetComponentInParent<Forward>();
            ElementActiveDebuff element = collision.GetComponentInParent<ElementActiveDebuff>();
            GameManager.Instance.FindStatName("slowedTime", Time.fixedDeltaTime);
            if (move != null)
            {
                move.path.maxSpeed = move.speedMax * 0.5f / dirtElement;
            }
            if (!element.IsActive("isDirt", true))
            {
                element.SetBool("isDirt", true, true);
                element.SetBool("isDirt", true, false);
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
