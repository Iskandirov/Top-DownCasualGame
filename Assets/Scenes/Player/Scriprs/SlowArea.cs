using UnityEngine;

public class SlowArea : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponentInParent<Forward>().speed = collision.GetComponentInParent<Forward>().speedMax * 0.5f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponentInParent<Forward>().speed = collision.GetComponentInParent<Forward>().speedMax;
        }
    }
}
