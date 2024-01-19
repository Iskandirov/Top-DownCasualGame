using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealActive : SkillBaseMono
{
    public PlayerManager player;
    public bool isTriggered;
    public float Grass;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        Grass = player.Grass;
        //basa = SetToSkillID(gameObject);
        if (isTriggered)
        {
            player.isInvincible = true;
        }
        if (player.playerHealthPoint != player.playerHealthPointMax)
        {
            if (player.playerHealthPoint + basa.damage * Grass <= player.playerHealthPointMax)
            {
                player.playerHealthPoint += basa.damage * Grass;
                player.fullFillImage.fillAmount += (basa.damage * Grass) / player.playerHealthPointMax;
                GameManager.Instance.FindStatName("healthHealed", basa.damage * Grass);
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
        yield return new WaitForSeconds(basa.lifeTime);

        if (isTriggered)
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
        GetComponent<Animator>().SetBool("IsShowed", true);
    }
}
