using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SniperTreeBullet : MonoBehaviour
{
    public float damage;
    public string[] targetPrefabNames;
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string targetPrefabName in targetPrefabNames)
        {
            Debug.Log(2);
            if (collision.gameObject.name == targetPrefabName && !collision.isTrigger)
            {
                collision.GetComponent<Health>().playerHealthPoint -= damage;
                collision.GetComponent<Health>().playerHealthPointImg.MinusProgressBar(damage);
                collision.GetComponent<Animator>().SetBool("IsHit", true);
                Destroy(gameObject);
            }
        }
    }
}
