using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    PlayerManager player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && player.playerHealthPoint < player.playerHealthPointMax)
        {
            if (player.playerHealthPoint + (player.playerHealthPointMax / 100) * 10 >= player.playerHealthPointMax)
            {
                player.playerHealthPoint = player.playerHealthPointMax;
                player.fullFillImage.fillAmount = 1;
                Destroy(gameObject);
            }
            else
            {
                player.playerHealthPoint += (player.playerHealthPointMax / 100) * 10; //Can be baffed by some thing
                player.fullFillImage.fillAmount = player.playerHealthPoint / player.playerHealthPointMax;
                Destroy(gameObject);
            }
        }
    }
}
