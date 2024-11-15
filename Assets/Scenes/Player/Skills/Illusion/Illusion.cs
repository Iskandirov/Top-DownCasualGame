using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX.Utility;

public class Illusion : SkillBaseMono
{
    public Zzap zzap;
    public Bullet bullet;
    public float x = -10;
    public float y = -7;
    public float xZzap;
    public float yZzap;
    public float angle;

    public float attackSpeed;
    public float attackSpeedMax;

    public bool isClone;
    Transform objTransform;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objTransform = transform;
        if (basa.stats[1].isTrigger)
        {
            basa.countObjects += basa.stats[1].value;
            basa.stats[1].isTrigger = false;
            if (isClone == false && FindObjectsOfType<Illusion>().Length < basa.countObjects)
            {
                CreateClone();
            }
        }
        if (basa.stats[2].isTrigger)
        {
            basa.lifeTime += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.countObjects += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
            if (isClone == false && FindObjectsOfType<Illusion>().Length < basa.countObjects)
            {
                CreateClone();
            }
        }
        if (basa.stats[4].isTrigger)
        {
            basa.stats[4].isTrigger = true;
        }
       
        attackSpeed = player.attackSpeed / player.Wind;
        attackSpeedMax = attackSpeed;
        if (basa.stats[4].isTrigger)
        {
            Zzap a = Instantiate(zzap, objTransform.position, Quaternion.Euler(0, 0, angle), objTransform);
            a.x = xZzap;
            a.y = yZzap;
            a.electicElement = player.Electricity;
            a.lifeTime = basa.lifeTime;
            GameObject b = a.GetComponent<VFXPropertyBinder>().m_Bindings[0].gameObject;
            Debug.Log(b.name);
        }
        CoroutineToDestroy(gameObject, basa.lifeTime);
        IsThereAnotherBeam();

    }
    public void IsThereAnotherBeam()
    {
        int x = -10;
        int y = -7;
        int xZzap = -5;
        int yZzap = 5;
        bool isReverce = true;
        List<Illusion> illusions = FindObjectsOfType<Illusion>().ToList();
        if (illusions.Count > 1)
        {
            foreach (Illusion illusion in illusions)
            {
                illusion.angle = angle;
                illusion.x = x;
                illusion.y = y;
                illusion.xZzap = xZzap;
                illusion.yZzap = yZzap;
                x += 10;
                y += isReverce ? 17 : -17;
                xZzap += 10;
                yZzap += isReverce ? 17 : -17;
                isReverce = !isReverce;
            }
            if (illusions.Count > 3)
            {
                Destroy(illusions[illusions.Count - 1].gameObject);
            }
        }
    }
    private void CreateClone()
    {
        Illusion a = Instantiate(this, objTransform.position, Quaternion.identity);
        a.isClone = true;
        a.basa.lifeTime = basa.lifeTime;
        a.basa.stats[4].isTrigger = basa.stats[4].isTrigger;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        attackSpeed -= Time.fixedDeltaTime;
        objTransform.position = new Vector2(player.ShootPoint.transform.position.x + x, player.ShootPoint.transform.position.y + y);
        if (attackSpeed <= 0 && Input.GetMouseButton(0) && !player.isAuto)
        {
            Bullet a = Instantiate(bullet, objTransform.position, Quaternion.identity);
            a.obj = gameObject;
            attackSpeed = attackSpeedMax;
        }
        else if(attackSpeed <= 0 && player.isAuto)
        {
            Bullet a = Instantiate(bullet, objTransform.position, Quaternion.identity);
            a.obj = gameObject;
            attackSpeed = attackSpeedMax;
        }
    }
}
