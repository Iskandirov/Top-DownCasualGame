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
        player = FindObjectOfType<Health>();

        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        if (isFiveLevel)
        {
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
