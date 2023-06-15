using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponentInChildren<LightOn>().IsPillBuffed == true)
            {
                collision.gameObject.GetComponentInChildren<LightOn>().step += collision.gameObject.GetComponentInChildren<LightOn>().stepMax;
                collision.gameObject.GetComponentInChildren<LightOn>().IsPillUp = true;
                collision.gameObject.GetComponentInChildren<LightOn>().IsPillBuffed = true;
            }
            else 
            {
                collision.gameObject.GetComponentInChildren<LightOn>().IsPillUp = true;
                collision.gameObject.GetComponentInChildren<LightOn>().IsPillBuffed = false;
            }
            
            
            Destroy(gameObject);
        }
    }
}
