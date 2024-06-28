using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fullFillImage;
    public SpriteRenderer buffArea;
    public float timeNeeded;
    public bool isNeedToMove;
    public bool playerInZone;
    public List<bool> buffTypes;
    PlayerManager player;
    

    private bool isBuffApplied;
    public void Start()
    {
        player = PlayerManager.instance;
        timeNeeded *= player.GivePerkStatValue(Stats.LoadSpeed) / 100;
    }
    public void Update()
    {
        if (!isNeedToMove)
        {
            BuffType();
            MinusProgressBar();
        }
    }
    public void PlusProgressBar()
    {
        if (isNeedToMove)
        {
            fullFillImage.fillAmount += 1 / timeNeeded * Time.deltaTime;
        }
        if (fullFillImage.fillAmount == 1)
        {
            buffArea.color = new Color32(255, 240, 117, 87);
            isNeedToMove = false;
        }
    } 
    public void MinusProgressBar()
    {
        if (!isNeedToMove)
        {
            fullFillImage.fillAmount -= 1 / timeNeeded * Time.deltaTime;
        }
        if (fullFillImage.fillAmount == 0)
        {
            buffArea.color = new Color32(255, 255, 255, 87);
            isNeedToMove = true;
            DeactivateBuff();
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            PlusProgressBar();
            if(buffArea.color != new Color32(255, 240, 117, 87))
            {
                isNeedToMove = true;
            }
            if (!isNeedToMove)
            {
                playerInZone = true;
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            isNeedToMove = false;
            playerInZone = false;
            DeactivateBuff();
        }
    }
    public void BuffType()
    {
        if (playerInZone)
        {
            if (!isBuffApplied)
            {
                if (buffTypes[0])
                {
                    player.attackSpeed = player.attackSpeedMax * 0.8f;
                }
                else if (buffTypes[1])
                {
                    player.damageToGive = player.damageToGive * 2f;
                }
                else if (buffTypes[2])
                {
                    player.playerHealthRegeneration = player.playerHealthRegeneration * 2f + 2f;
                }
                else if (buffTypes[3])
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
                else if (buffTypes[4])
                {
                    player.multiply = 2;
                }

                isBuffApplied = true;
            }
        }
    }
    public void DeactivateBuff()
    {
        if (isBuffApplied)
        {
            isBuffApplied = false;
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
                player.playerHealthRegeneration = player.playerHealthRegeneration - 2f / 2f;
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
}
