using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float damage;
    public string[] targetPrefabNames;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Invoke("DestroyBullet", 3f);
    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string targetPrefabName in targetPrefabNames)
        {
            if (collision.gameObject.name == targetPrefabName && !collision.isTrigger)
            {
                collision.GetComponent<Health>().playerHealthPoint -= damage;
                collision.GetComponent<Health>().playerHealthPointImg.fillAmount -= damage / collision.GetComponent<Health>().playerHealthPointMax;
                collision.GetComponent<Animator>().SetBool("IsHit", true);
                Destroy(gameObject);
            }
        }
    }
}
