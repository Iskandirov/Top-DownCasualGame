using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    Animator anim;
    public CineMachineCameraShake vCam;
    // Start is called before the first frame update
    void Start()
    {
        vCam = GameObject.FindGameObjectWithTag("Respawn").GetComponent<CineMachineCameraShake>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            anim.SetTrigger("Triggered");
            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();
            player.TakeDamage(player.playerHealthPointMax * 0.1f);
            vCam.Shake(20f, 0.2f);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            anim.SetTrigger("Triggered");
        }
    }
}
