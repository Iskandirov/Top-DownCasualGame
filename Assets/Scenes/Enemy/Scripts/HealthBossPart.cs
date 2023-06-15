using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBossPart : MonoBehaviour
{
    public float step;
    public float stepMax;
    public bool isKick;
    public float healthPoint;
    public float healthPointMax;
    public bool IsBobs;
    public Animator anim;
    public ElementalBoss_Destroy bodyAnim;
    public bool isBurn;
    public float burnTime;
    public float burnDamage;
    public float burnTick;
    public float burnTickMax;
    CutThePart objPart;
    SpriteRenderer[] objsSprite;

    void Start()
    {
        healthPointMax = healthPoint;
        objPart = GetComponent<CutThePart>();
        objsSprite = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (isBurn)
        {
            burnTime -= Time.deltaTime;

            if (burnTime <= 0)
            {
                isBurn = false;
            }

            burnTick -= Time.deltaTime;

            if (burnTick <= 0)
            {
                ChangeToKick();
                healthPoint -= burnDamage;
                burnTick = burnTickMax;
            }
        }
        else
        {
            ChangeToNotKick();
        }

        if (healthPoint <= 0)
        {
            if (bodyAnim != null)
            {
                foreach (var part in bodyAnim.parts)
                {
                    if (part == gameObject.name)
                    {
                        bodyAnim.DestroyStart();
                        bodyAnim.GetParts(objPart, healthPointMax);
                    }
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void ChangeToKick()
    {
        if (!isKick)
        {
            for (int i = 0; i < objsSprite.Length; i++)
            {
                objsSprite[i].color = new Color32(255, 0, 0, 255);
            }
            isKick = true;
        }
    }

    public void ChangeToNotKick()
    {
        if (isKick)
        {
            step -= Time.deltaTime;

            if (step <= 0)
            {
                for (int i = 0; i < objsSprite.Length; i++)
                {
                    objsSprite[i].color = new Color32(255, 255, 255, 255);
                }
                isKick = false;
                step = stepMax;
            }
        }
    }
}
