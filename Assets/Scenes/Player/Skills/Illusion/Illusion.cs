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
    public float angle;

    public float attackSpeed;
    public float attackSpeedMax;
   
    Transform objTransform;
    public List<GameObject> illusions = new List<GameObject>();
    public bool isClone = false;
    public Vector3[] initialOffsets = new Vector3[3];
    public Vector3[] innerInitialOffsets = new Vector3[3];
    //Vector3[] innerDir = new Vector3[3];
    public List<GameObject> innerIllusions = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objTransform = transform;
        illusions.Add(this.gameObject);
        if (basa.stats[1].isTrigger && !isClone)
        {
            Illusion a = Instantiate(this);
            a.isClone = true;
            illusions.Add(a.gameObject);
            
        }
        if (basa.stats[2].isTrigger)
        {
            basa.lifeTime += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger && !isClone)
        {
            Illusion a = Instantiate(this);
            a.isClone = true;
            illusions.Add(a.gameObject);
        }
        attackSpeed = player.attackSpeed / player.Wind;
        attackSpeedMax = attackSpeed;
        CoroutineToDestroy(gameObject, basa.lifeTime);
        if (!isClone)
        {
            IllusionPosition();
        }
    }
    void IllusionPosition()
    {
        for (int i = 0; i < illusions.Count; i++)
        {
            float angle = i * 2 * Mathf.PI / illusions.Count;
            initialOffsets[i] = new Vector2(Mathf.Cos(angle) * 15, Mathf.Sin(angle) * 15) + new Vector2(0, 10f); //Вираховую об'єкти по колу навколо гравця + похибка через зміщення гравця
            illusions[i].transform.position = player.objTransform.position + initialOffsets[i];
            illusions[i].GetComponent<Illusion>().initialOffsets = initialOffsets;
            //innerDir[i] = illusions[i].transform.position - player.objTransform.position;
            //illusions[i].transform.parent = player.transform;

        }

        if (basa.stats[4].isTrigger)
        {
            for (int i = 0; i < illusions.Count; i++)
            {
                int nextIndex = (i + 1) % illusions.Count; // Індекс наступного об'єкта (для зациклення)

                // Обчислення середньої точки між двома зовнішніми об'єктами
                Vector3 midpoint = (illusions[i].transform.position + illusions[nextIndex].transform.position) / 2f;
                innerInitialOffsets[i] = midpoint - player.objTransform.position;
                Zzap innerIllusion = Instantiate(zzap);
                innerIllusion.lifeTime = basa.lifeTime;
                innerIllusion.basa = basa;
                innerIllusion.transform.position = midpoint;
                Vector2 targetDirection = illusions[i].transform.position - innerIllusion.transform.position;
                float angleZ = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                innerIllusion.transform.rotation = Quaternion.Euler(0f, 0f, angleZ);
                innerIllusions.Add(innerIllusion.gameObject);
                //innerIllusion.transform.parent = player.transform;
            }
        }
    }  
    void FixedUpdate()
    {
        // Оновлення позицій внутрішніх об'єктів
        for (int i = 0; i < illusions.Count; i++)
        {
            illusions[i].transform.position = player.objTransform.position + initialOffsets[i];
        }
        for (int i = 0; i < innerIllusions.Count; i++)
        {
            innerIllusions[i].transform.position = player.objTransform.position + innerInitialOffsets[i];
        }

        attackSpeed -= Time.fixedDeltaTime;
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
