using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBarrier : MonoBehaviour
{
    public float lifeTime;

    public Health player;
    public bool isFiveLevel;
    public float heal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            if (isFiveLevel)
            {
                player = FindObjectOfType<Health>();

                if (ReferenceEquals(player.gameObject, gameObject))
                {

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
            }
            Destroy(gameObject);
        }
    }
}
