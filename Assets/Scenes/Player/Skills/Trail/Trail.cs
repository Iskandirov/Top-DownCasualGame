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
[RequireComponent(typeof(EdgeCollider2D))]
public class Trail : SkillBaseMono
{
    public VisualEffect vfx;
    public TrailRenderer trailRenderer;
    public EdgeCollider2D edgeCollider;
    public static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();
    public float size;
    EdgeCollider2D validCollider;
    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        edgeCollider = GetValidCollider();
        player = PlayerManager.instance;
        StartCoroutine(PeriodicSpawn());
    }
    private void FixedUpdate()
    {
        if (basa.stats[1].isTrigger)
        {
            basa.lifeTime += basa.stats[1].value;
            trailRenderer.time = basa.lifeTime;
            validCollider.GetComponent<TriggerTrail>().basa = basa;
            basa.stats[1].isTrigger = false;
        }
        else if (basa.stats[2].isTrigger)
        {
            basa.damage += basa.stats[2].value;
            validCollider.GetComponent<TriggerTrail>().basa = basa;
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
        while (true)
        {
            SetColliderPointsFromTrail(trailRenderer, edgeCollider);
            yield return new WaitForSeconds(.5f);
        }
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
        collider.offset = new Vector2(0, 5f);
    }
    EdgeCollider2D GetValidCollider()
    {
        if (unusedColliders.Count > 0)
        {
            validCollider = unusedColliders[0];
            validCollider.enabled = true;
            unusedColliders.RemoveAt(0);
        }
        else
        {
            validCollider = new GameObject("TrailCollder", typeof(EdgeCollider2D)).GetComponent<EdgeCollider2D>();
            validCollider.AddComponent<TriggerTrail>();
            validCollider.GetComponent<TriggerTrail>().basa = basa;
            validCollider.gameObject.layer = 7;
            validCollider.isTrigger = true;
        }
        return validCollider;
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
}
