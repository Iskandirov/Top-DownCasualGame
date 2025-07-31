using FSMC.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Trail : SkillBaseMono
{
    public VisualEffect vfx;
    public TrailRenderer trailRenderer;
    public BoxCollider2D edgeCollider;
    //public static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();
    public float size;
    //EdgeCollider2D validCollider;
    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        player = PlayerManager.instance;
        StartCoroutine(PeriodicSpawn());
    }
    private void FixedUpdate()
    {
        if (basa.stats[1].isTrigger)
        {
            basa.lifeTime += basa.stats[1].value;
            trailRenderer.time = basa.lifeTime;
            //validCollider.GetComponent<TriggerTrail>().basa = basa;
            basa.stats[1].isTrigger = false;
        }
        else if (basa.stats[2].isTrigger)
        {
            basa.damage += basa.stats[2].value;
            //validCollider.GetComponent<TriggerTrail>().basa = basa;
            basa.stats[2].isTrigger = false;
        }
        else if (basa.stats[3].isTrigger)
        {
            size += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }
        vfx.SetVector3("PlayerPosition", player.objTransform.position + new Vector3(0,5f,0));
    }

    IEnumerator PeriodicSpawn()
    {
        Vector3 lastPos = transform.position;

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Vector3 currentPos = transform.position;
            float distance = Vector3.Distance(currentPos, lastPos);

            if (distance > 0.5f)
            {
                CreateColliderSegment(lastPos, currentPos);
                lastPos = currentPos;
            }
        }
    }
    void CreateColliderSegment(Vector3 start, Vector3 end)
    {
        Vector3 midPoint = (start + end) / 2f;
        Vector2 dir = end - start;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject segment = new GameObject("TrailSegment");
        segment.transform.position = midPoint;
        segment.transform.rotation = Quaternion.Euler(0, 0, angle);

        BoxCollider2D box = segment.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(distance, size);  // Довжина — уздовж шляху, ширина — товщина сліду

        // Обов’язково обнули зміщення, щоб центр був на середині
        box.offset = Vector2.zero;

        //segment.layer = LayerMask.NameToLayer("Trail");

        TriggerTrail trail = segment.AddComponent<TriggerTrail>();
        trail.basa = basa;
        trail.Elements = Elements;

        Destroy(segment, trailRenderer.time);
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
            //unusedColliders.Add(edgeCollider);
        }
    }
}
