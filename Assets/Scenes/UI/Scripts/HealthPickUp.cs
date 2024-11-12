using System.Linq;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    PlayerManager player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
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
            DailyQuests.instance.UpdateValue(1, (player.playerHealthPointMax / 100) * 10, false);
            DailyQuests.instance.UpdateValue(5, 1, false);
        }
    }
}
