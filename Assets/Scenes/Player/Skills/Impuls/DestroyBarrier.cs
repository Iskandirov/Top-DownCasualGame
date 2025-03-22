using System.Collections;
using System.Linq;
using UnityEngine;

public class DestroyBarrier : MonoBehaviour
{
    public float lifeTime;

    PlayerManager player;
    public bool isFiveLevel;
    public float heal;
    public float Grass;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;

        StartCoroutine(TimerSpell());
    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        if (isFiveLevel)
        {

            if (player.playerHealthPoint != player.playerHealthPointMax)
            {
                if (player.playerHealthPoint + heal * Grass <= player.playerHealthPointMax)
                {
                    if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 1 && s.isActive == true) != null)
                    {
                        DailyQuests.instance.UpdateValue(1, heal * Grass, false, true);
                    }
                        player.playerHealthPoint += heal * Grass;
                    GameManager.Instance.fullFillImage.fillAmount += player.playerHealthPoint / player.playerHealthPointMax;
                }
                else
                {
                    if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 1 && s.isActive == true) != null)
                    {
                        DailyQuests.instance.UpdateValue(1, heal * Grass, false, true);
                    }
                        player.playerHealthPoint = player.playerHealthPointMax;
                    GameManager.Instance.fullFillImage.fillAmount -= player.playerHealthPoint / player.playerHealthPointMax;
                }
            }
        }
        Destroy(gameObject);

    }
}
