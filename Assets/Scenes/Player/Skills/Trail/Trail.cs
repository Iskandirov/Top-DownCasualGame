using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Trail : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public EdgeCollider2D edgeCollider;

    public float damage;
    public float size;

    static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();
    PlayerManager player;
    public bool isFive;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        trailRenderer = GetComponent<TrailRenderer>();

        edgeCollider = GetValidCollider();
    }
    private void FixedUpdate()
    {
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
                    colliders[j].collider.GetComponent<HealthPoint>().healthPoint -= damage;
                    if (isFive)
                    {
                        if (colliders[j].collider.GetComponent<HealthPoint>().healthPoint <= 0)
                        {
                            player.playerHealthPoint += colliders[j].collider.GetComponent<HealthPoint>().healthPointMax * 0.1f;
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
        if (edgeCollider != null)
        {
            edgeCollider.enabled = false;
            unusedColliders.Add(edgeCollider);
        }
    }
}
