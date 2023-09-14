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
    GameObject player;
    

    private bool isBuffApplied;
    public void Start()
    {
        player = FindObjectOfType<Move>().gameObject;
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
                    player.GetComponent<Shoot>().attackSpeed = player.GetComponent<Shoot>().attackSpeedMax * 0.8f;
                }
                else if (buffTypes[1])
                {
                    player.GetComponent<Shoot>().damageToGive = player.GetComponent<Shoot>().damageToGive * 2f;
                }
                else if (buffTypes[2])
                {
                    player.GetComponent<Health>().playerHealthRegeneration = player.GetComponent<Health>().playerHealthRegeneration * 2f + 2f;
                }
                else if (buffTypes[3])
                {
                    ElementsCoeficients objCoef = player.GetComponent<ElementsCoeficients>();
                    objCoef.Fire = objCoef.Fire * 2f;
                    objCoef.Electricity = objCoef.Electricity * 2f;
                    objCoef.Water = objCoef.Water * 2f;
                    objCoef.Dirt = objCoef.Dirt * 2f;
                    objCoef.Wind = objCoef.Wind * 2f;
                    objCoef.Grass = objCoef.Grass * 2f;
                    objCoef.Steam = objCoef.Steam * 2f;
                    objCoef.Cold = objCoef.Cold * 2f;
                }
                else if (buffTypes[4])
                {
                    player.GetComponent<Expirience>().multiply = 2;
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
                player.GetComponent<Shoot>().attackSpeed = player.GetComponent<Shoot>().attackSpeedMax;

            }
            else if (buffTypes[1])
            {
                player.GetComponent<Shoot>().damageToGive = player.GetComponent<Shoot>().damageToGive / 2f;
            }
            else if (buffTypes[2])
            {
                player.GetComponent<Health>().playerHealthRegeneration = player.GetComponent<Health>().playerHealthRegeneration - 2f / 2f;
            }
            else if (buffTypes[3])
            {
                ElementsCoeficients objCoef = player.GetComponent<ElementsCoeficients>();
                objCoef.Fire = objCoef.Fire / 2f;
                objCoef.Electricity = objCoef.Electricity / 2f;
                objCoef.Water = objCoef.Water / 2f;
                objCoef.Dirt = objCoef.Dirt / 2f;
                objCoef.Wind = objCoef.Wind / 2f;
                objCoef.Grass = objCoef.Grass / 2f;
                objCoef.Steam = objCoef.Steam / 2f;
                objCoef.Cold = objCoef.Cold / 2f;
            }
            else if (buffTypes[4])
            {
                player.GetComponent<Expirience>().multiply = 1;
            }
        }
    }
}
