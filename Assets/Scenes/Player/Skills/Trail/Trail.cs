using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Trail : SkillBaseMono
{
    public TrailRenderer trailRenderer;
    public EdgeCollider2D edgeCollider;

    public float size;

    public static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();
    PlayerManager player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.startWidth = size;
        trailRenderer.endWidth = size;
        edgeCollider = GetValidCollider();
    }
    private void FixedUpdate()
    {
        if (basa.stats[1].isTrigger)
        {
            basa.lifeTime += basa.stats[1].value;
            trailRenderer.time = basa.lifeTime;
            basa.stats[1].isTrigger = false;
        }
        if (basa.stats[2].isTrigger)
        {
            basa.damage += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        if (basa.stats[3].isTrigger)
        {
            basa.radius += basa.stats[3].value;
            size += basa.radius;
            trailRenderer.startWidth = size;
            trailRenderer.endWidth = size;
            basa.stats[3].isTrigger = false;
        }
        transform.position = player.transform.position;
        SetColliderPointsFromTrail(trailRenderer, edgeCollider);
        //Trail
        for (int i = 0; i < trailRenderer.positionCount; i++)
        {
            // створюємо промінь, який виходить з точки в TrailRenderer
            Ray2D ray = new Ray2D(trailRenderer.GetPosition(i), trailRenderer.transform.forward);

            // використовуємо метод Physics.RaycastAll() для отримання масиву Colliders, які перетинаються з променем
            RaycastHit2D[] colliders = Physics2D.RaycastAll(ray.origin, ray.direction);

            // перевіряємо, чи є в масіві Colliders об'єкт з тегом "Enemy"
            for (int j = 0; j < colliders.Length; j++)
            {
                if (colliders[j].collider.CompareTag("Enemy"))
                {
                    // TrailRenderer торкається до об'єкта з тегом "Enemy"
                    colliders[j].collider.GetComponent<HealthPoint>().healthPoint -= basa.damage;
                    if (basa.stats[4].isTrigger)
                    {
                        if (colliders[j].collider.GetComponent<HealthPoint>().healthPoint <= 0)
                        {
                            player.playerHealthPoint += colliders[j].collider.GetComponent<HealthPoint>().healthPointMax * 0.1f;
                            player.fullFillImage.fillAmount = player.playerHealthPoint / player.playerHealthPointMax;
                        }
                    }

                    return;
                }
            }
        }
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
        // Видаляємо колайдер тільки якщо він був створений в методі GetValidCollider()
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
