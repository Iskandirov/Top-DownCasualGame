using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Root_Bullet : MonoBehaviour
{
    public GameObject objectToLeave; 
    public Transform leavePoint; 
    public float delay; 
    float delayMax;
    public float damage;
    public float lifeTime;

    public void Start()
    {
        delayMax = delay;
        Invoke("DestroObj", lifeTime);
    }
    private void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
        {
            delay = delayMax;
            Instantiate(objectToLeave, leavePoint.position, Quaternion.identity);
        }
    }
    public void DestroObj()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !collision.collider.isTrigger)
        {
            collision.collider.GetComponent<Health>().playerHealthPoint -= damage;
            collision.collider.GetComponent<Health>().playerHealthPointImg.fullFillImage.fillAmount -= damage / collision.collider.GetComponent<Health>().playerHealthPointMax;
            collision.collider.GetComponent<Health>().GetComponent<Animator>().SetBool("IsHit", true);
            collision.collider.GetComponent<Move>().isSlowingDown = true;
            collision.collider.GetComponent<Move>().slowdownEndTime = 4f;
            collision.collider.GetComponent<Move>().slowPercent = 0.2f;
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
