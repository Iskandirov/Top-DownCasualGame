using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fullFillImage;
    public SpriteRenderer buffArea;
    public List<bool> buffTypes;
    PlayerManager player;


    private bool isBuffApplied;
    public void Start()
    {
        player = PlayerManager.instance;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            Instantiate(player.buffObj, player.transform.position, Quaternion.identity, player.objTransform);
            BuffType();
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            Instantiate(player.debuffObj, player.transform.position, Quaternion.identity, player.objTransform);
            DeactivateBuff();
        }
    }
    public void BuffType()
    {
        if (buffTypes[0])
        {
            player.attackSpeed = player.attackSpeedMax * 0.8f;
        }
        if (buffTypes[1])
        {
            player.damageToGive = player.damageToGive * 2f;
        }
        if (buffTypes[2])
        {
            player.playerHealthRegeneration = (player.playerHealthRegeneration + 1f) * 2f;
        }
        if (buffTypes[3])
        {
            player.Fire = player.Fire * 2f;
            player.Electricity = player.Electricity * 2f;
            player.Water = player.Water * 2f;
            player.Dirt = player.Dirt * 2f;
            player.Wind = player.Wind * 2f;
            player.Grass = player.Grass * 2f;
            player.Steam = player.Steam * 2f;
            player.Cold = player.Cold * 2f;
        }
        if (buffTypes[4])
        {
            player.multiply = 2;
        }

    }
    public void DeactivateBuff()
    {
        if (buffTypes[0])
        {
            player.attackSpeed = player.attackSpeedMax;

        }
        else if (buffTypes[1])
        {
            player.damageToGive = player.damageToGive / 2f;
        }
        else if (buffTypes[2])
        {
            player.playerHealthRegeneration = player.playerHealthRegeneration / 2f - 1f;
        }
        else if (buffTypes[3])
        {
            player.Fire = player.Fire / 2f;
            player.Electricity = player.Electricity / 2f;
            player.Water = player.Water / 2f;
            player.Dirt = player.Dirt / 2f;
            player.Wind = player.Wind / 2f;
            player.Grass = player.Grass / 2f;
            player.Steam = player.Steam / 2f;
            player.Cold = player.Cold / 2f;
        }
        else if (buffTypes[4])
        {
            player.multiply = 1;
        }
    }
}
