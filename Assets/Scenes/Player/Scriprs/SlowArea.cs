using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.transform.root.GetComponent<Forward>().speed = collision.transform.root.GetComponent<Forward>().speedMax * 0.5f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.transform.root.GetComponent<Forward>().speed = collision.transform.root.GetComponent<Forward>().speedMax;
        }
    }
}
