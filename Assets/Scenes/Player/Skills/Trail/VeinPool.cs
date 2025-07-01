using FSMC.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class VeinPool : MonoBehaviour
{
    public List<GrowMat_Script> objType;
    public List<GameObject> growPool;
    public Transform trailParent;
    public float spawnDelay = 0.5f;

    public int index = 0;
    int count = 2;
    public SkillBase skillData;
    PlayerManager player;

    public float size;

    public AnimationCurve myCurve;
    public TrailRenderer trailRenderer;
    public EdgeCollider2D edgeCollider;
    public static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();
    public Transform objTransform;
    public float hitDelay = 0.5f;

    private void Start()
    {
        if (trailParent != null) 
        {
            foreach (var vein in objType)
            {
                vein.trailParent = trailParent;
            }
        }
        player = PlayerManager.instance;

        StartCoroutine(GrowTrail());
        objTransform = transform;
        trailRenderer = GetComponent<TrailRenderer>();

        Keyframe key = myCurve.keys[1];
        key.value = size;
        myCurve.MoveKey(1, key);
        trailRenderer.widthCurve = myCurve;

        edgeCollider = GetValidCollider();
    }

    private void FixedUpdate()
    {
        if (skillData.stats[1].isTrigger)
        {
            skillData.lifeTime += skillData.stats[1].value;
            //trailRenderer.time = basa.lifeTime;
            skillData.stats[1].isTrigger = false;
        }
        if (skillData.stats[2].isTrigger)
        {
            skillData.damage += skillData.stats[2].value;
            skillData.stats[2].isTrigger = false;
        }
        if (skillData.stats[3].isTrigger)
        {
            skillData.damageTickMax -= skillData.stats[3].value;
            //size += basa.radius;
            //trailRenderer.startWidth = size;
            //trailRenderer.endWidth = size;
            skillData.stats[3].isTrigger = false;
        }
        objTransform.position = player.ShootPoint.transform.position;
        SetColliderPointsFromTrail(trailRenderer, edgeCollider);
        //Trail
        for (int i = 0; i < trailRenderer.positionCount; i++)
        {
            Ray2D ray = new Ray2D(trailRenderer.GetPosition(i), trailRenderer.transform.forward);

            RaycastHit2D[] colliders = Physics2D.RaycastAll(ray.origin, ray.direction);
            EnemySpawner enemies = FindAnyObjectByType<EnemySpawner>();
            for (int j = 0; j < colliders.Length; j++)
            {
                if (colliders[j].collider.CompareTag("Enemy") && hitDelay <= 0)
                {
                    ElementActiveDebuff debuff = colliders[j].collider.GetComponentInParent<ElementActiveDebuff>();
                    debuff.ApplyEffect(Elements.status.Grass, 5);

                    colliders[j].collider.GetComponent<FSMC_Executer>().TakeDamage(skillData.damage, 1);
                    if (skillData.stats[4].isTrigger)
                    {
                        if (colliders[j].collider.GetComponent<FSMC_Executer>().health <= 0)
                        {
                            float heal = enemies.children.Find(s => s.name == colliders[j].collider.GetComponent<FSMC_Executer>().name).healthMax * 0.1f;
                            if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 1 && s.isActive == true) != null)
                            {
                                DailyQuests.instance.UpdateValue(1, heal, false, true);
                            }
                            player.playerHealthPoint += heal;
                            GameManager.Instance.fullFillImage.fillAmount = player.playerHealthPoint / player.playerHealthPointMax;
                        }
                    }
                    hitDelay = 0.5f;
                    return;
                }
            }
        }
        hitDelay -= Time.fixedDeltaTime;
    }
    EdgeCollider2D GetValidCollider()
    {
        EdgeCollider2D validCollider;
        if (unusedColliders.Count > 0)
        {
            validCollider = unusedColliders[0];
            validCollider.enabled = true;
            unusedColliders.RemoveAt(0);
        }
        else
        {
            validCollider = new GameObject("TrailCollder", typeof(EdgeCollider2D)).GetComponent<EdgeCollider2D>();
            validCollider.isTrigger = true;
        }
        return validCollider;
    }
    public void SetColliderPointsFromTrail(TrailRenderer trail, EdgeCollider2D collider)
    {
        List<Vector2> points = new List<Vector2>();
        for (int position = 0; position < trail.positionCount; position++)
        {
            points.Add(trail.GetPosition(position));
        }
        collider.SetPoints(points);
        collider.edgeRadius = size;
    }
    private void OnDestroy()
    {
        if (edgeCollider.gameObject.name == "TrailCollder")
        {
            Destroy(edgeCollider.gameObject);
        }
        else
        {
            edgeCollider.enabled = false;
            unusedColliders.Add(edgeCollider);
        }
    }
    IEnumerator GrowTrail()
    {
        while (true)
        {
            skillData = trailParent.gameObject.GetComponent<Trail>().basa;
            if (index < growPool.Count)
            {
                for (int i = 0; i < count; i++)
                {
                    growPool[index + i].GetComponent<GrowMat_Script>().GetRotationAndPosition();
                    objType[index + i].timeToGrow = skillData.lifeTime;
                    objType[index + i].damage = skillData.damage;
                    objType[index + i].damageTick = skillData.damageTickMax;
                }
                index += count;
            }
            else
            {
                index = 0;
            }
            yield return new WaitForSeconds(spawnDelay);

        }
    }
}
