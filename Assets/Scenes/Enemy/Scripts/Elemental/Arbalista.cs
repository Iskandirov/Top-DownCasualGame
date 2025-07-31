using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arbalista : MonoBehaviour
{
    public bool TikTokBool;
    public Animator anim;
    public GameObject arBullet;
    public Transform spawnArBullet;
    public Transform mirrorTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (TikTokBool)
        {
            anim.SetTrigger("Charged");
        }
    }
    public void CreateArbullet()
    {
        GameObject bulletObj = Instantiate(arBullet, spawnArBullet.position, spawnArBullet.rotation);
        ArbalistaBullet bullet = bulletObj.GetComponent<ArbalistaBullet>();
        // Отримати напрямок з повороту арбалета
        Vector2 shootDirection = spawnArBullet.up; // або up, залежно від того, як повертається арбалет

        bullet.direction = shootDirection;

        Vector2 incoming = shootDirection.normalized;
        Vector2 mirrorNormal = mirrorTransform.up; // або right, залежно від обертання

        Vector2 reflected = Vector2.Reflect(incoming, mirrorNormal);

        //// Для відладки
        //Debug.DrawRay(spawnArBullet.position, incoming, Color.green, 1f);
        //Debug.DrawRay(spawnArBullet.position, mirrorNormal, Color.yellow, 1f);
        //Debug.DrawRay(spawnArBullet.position, reflected, Color.red, 1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
