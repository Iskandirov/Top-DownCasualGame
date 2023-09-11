using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealActive : MonoBehaviour
{
    public Health player;
    public float lifeTime;
    public float heal;
    public bool isLevelTwo;
    public float Grass;
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
            if (player.playerHealthPoint + heal * Grass <= player.playerHealthPointMax)
            {
                player.playerHealthPoint += heal * Grass;
                player.playerHealthPointImg.fullFillImage.fillAmount += (heal * Grass) / player.playerHealthPointMax;
            }
            else
            {
                player.playerHealthPoint = player.playerHealthPointMax;
                player.playerHealthPointImg.fullFillImage.fillAmount = 1f;
            }
        }
        StartCoroutine(TimerSpell());

    }

    private IEnumerator TimerSpell()
    {
        yield return new WaitForSeconds(lifeTime);

        if (isLevelTwo)
        {
            player.GetComponent<Move>().isInvincible = false;
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
