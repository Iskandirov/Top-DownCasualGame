using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealActive : MonoBehaviour
{
    public Health player;
    public float lifeTime;
    public float heal;
    public bool isLevelTwo;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Health>();
        if (isLevelTwo)
        {
            player.GetComponent<Move>().isInvincible = true;
        }
        if (player.playerHealthPoint != player.playerHealthPointMax)
        {
            if (player.playerHealthPoint + heal <= player.playerHealthPointMax)
            {
                player.playerHealthPoint += heal;
                player.playerHealthPointImg.fillAmount = player.playerHealthPoint / player.playerHealthPointMax;
            }
            else
            {
                player.playerHealthPoint = player.playerHealthPointMax;
                player.playerHealthPointImg.fillAmount = player.playerHealthPoint / player.playerHealthPointMax;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        lifeTime -= Time.deltaTime;
        if (lifeTime <=0)
        {
            if (isLevelTwo)
            {
                player.GetComponent<Move>().isInvincible = false;
            }
            Destroy(gameObject);
        }
    }
}
