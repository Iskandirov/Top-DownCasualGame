using System.Collections;
using UnityEngine;

public class HealActive : MonoBehaviour
{
    public PlayerManager player;
    public float lifeTime;
    public float heal;
    public bool isLevelTwo;
    public float Grass;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        if (isLevelTwo)
        {
            player.isInvincible = true;
        }
        if (player.playerHealthPoint != player.playerHealthPointMax)
        {
            if (player.playerHealthPoint + heal * Grass <= player.playerHealthPointMax)
            {
                player.playerHealthPoint += heal * Grass;
                player.fullFillImage.fillAmount += (heal * Grass) / player.playerHealthPointMax;
                GameManager.Instance.FindStatName("healthHealed", heal * Grass);
            }
            else
            {
                GameManager.Instance.FindStatName("healthHealed", player.playerHealthPointMax - player.playerHealthPoint);
                player.playerHealthPoint = player.playerHealthPointMax;
                player.fullFillImage.fillAmount = 1f;
            }
        }
        StartCoroutine(TimerSpell());

    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        if (isLevelTwo)
        {
            player.isInvincible = false;
        }
        Destroy(gameObject);

    }
    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
    }
    public void isShowed()
    {
        GetComponent<Animator>().SetBool("IsShowed",true);
    }
}
