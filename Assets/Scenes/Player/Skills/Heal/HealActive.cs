using System.Collections;
using UnityEngine;

public class HealActive : SkillBaseMono
{
    public PlayerManager player;
    public float Grass;
    Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objTransform = transform;
        Grass = player.Grass;
        if (basa.stats[1].isTrigger)
        {
            basa.damage += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.stepMax -= basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
        if (basa.stats[4].isTrigger)
        {
            basa.damage += basa.stats[4].value;
            basa.stats[4].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
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

        if (basa.stats[2].isTrigger)
        {
            player.isInvincible = false;
        }
        Destroy(gameObject);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        objTransform.position = player.objTransform.position;
    }
    public void isShowed()
    {
        GetComponent<Animator>().SetBool("IsShowed", true);
    }
}
