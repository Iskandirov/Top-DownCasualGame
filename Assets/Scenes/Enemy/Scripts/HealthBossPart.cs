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
    public GameObject bodyAnim;
    public bool isBurn;
    public float burnTime;
    public float burnDamage;
    public float burnTick;
    public float burnTickMax;
    // Start is called before the first frame update
    void Start()
    {
        healthPointMax = healthPoint;
    }

    // Update is called once per frame
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
            if (burnTick <=0)
            {
                ChangeToKick();
                healthPoint -= burnDamage;
                burnTick = burnTickMax;
            }
            
        }
        ChangeToNotKick();
        if (healthPoint <= 0)
        {
            if (bodyAnim != null)
            {
                foreach (var part in bodyAnim.GetComponent<ElementalBoss_Destroy>().parts)
                {
                    if (part == gameObject.name)
                    {
                        bodyAnim.GetComponent<ElementalBoss_Destroy>().DestroyStart();
                        bodyAnim.GetComponent<ElementalBoss_Destroy>().GetParts(gameObject.GetComponent<CutThePart>(), healthPointMax);
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
        for (int i = 0; i < gameObject.transform.GetComponentsInChildren<SpriteRenderer>().Length; i++)
        {
            gameObject.transform.GetComponentsInChildren<SpriteRenderer>()[i].color = new Color32(255, 0, 0, 255);
        }
        isKick = true;
    }
    public void ChangeToNotKick()
    {
        if (isKick == true)
        {
            step -= Time.deltaTime;
            if (step <= 0)
            {
                for (int i = 0; i < gameObject.transform.GetComponentsInChildren<SpriteRenderer>().Length; i++)
                {
                    gameObject.transform.GetComponentsInChildren<SpriteRenderer>()[i].color = new Color32(255, 255, 255, 255);
                }
                isKick = false;
                step = stepMax;
            }
        }
    }
}
