using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HealthPoint : MonoBehaviour
{
    public GameObject player;
    public float stepKick;
    public float stepKickMax;
    public bool isKick;
    public float healthPoint;
    public float healthPointMax;
    public EXP expiriancePoint;
    public bool IsBobs;
    public Animator anim;
    public GameObject bodyAnim;
    public GameObject VFX_Deadarticle;
    public bool isBurn;
    public float burnTime;
    public float burnDamage;
    public float burnTick;
    public float burnTickMax;
    SpriteRenderer[] objsSprite;
    Expirience objExp;
    DropItems objDrop;
    Forward objMove;
    KillCount objKills;
    // Start is called before the first frame update
    void Start()
    {
        healthPointMax = healthPoint;
        player = GameObject.FindWithTag("Player");
        objsSprite = GetComponentsInChildren<SpriteRenderer>();
        objExp = player.GetComponent<Expirience>();
        objDrop = GetComponentInParent<DropItems>();
        objMove = GetComponentInParent<Forward>();
        objKills = FindObjectOfType<KillCount>();
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
            if (burnTick <= 0)
            {
                ChangeToKick();
                healthPoint -= burnDamage;
                ChangeToNotKick();
                burnTick = burnTickMax;
            }

        }
        ChangeToNotKick();
        if (healthPoint <= 0)
        {
            EXP a = Instantiate(expiriancePoint, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1.9f), Quaternion.identity);
            a.expBuff = healthPointMax / 5;

            objExp.expiriencepoint.fillAmount += healthPointMax / (objExp.level * 10);
            objExp.ShowLevel();
            objKills.score += 1;

            if (IsBobs)
            {
                objDrop.OnDestroyBoss();
                Destroy(gameObject);
                
            }

            objMove.transform.position = new Vector2(0, 0);

            healthPoint = healthPointMax;
        }
    }
    
    public void ChangeToKick()
    {
        for (int i = 0; i < objsSprite.Length; i++)
        {
            objsSprite[i].color = new Color32(255, 0, 0, 255);
        }
        isKick = true;
    } 
    public void ChangeToNotKick()
    {
        if (isKick == true)
        {
            stepKick -= Time.deltaTime;
            if (stepKick <= 0)
            {
                for (int i = 0; i < objsSprite.Length; i++)
                {
                    objsSprite[i].color = new Color32(255, 255, 255, 255);
                }
                isKick = false;
                stepKick = stepKickMax;
            }
        }
    }
}
