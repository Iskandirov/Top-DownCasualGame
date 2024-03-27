using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Trail : SkillBaseMono
{
    public TrailRenderer trailRenderer;
    public EdgeCollider2D edgeCollider;

    public float size;

    public float hitDelay = 0.5f;

    public static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();
    public Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        objTransform = transform;
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
        objTransform.position = player.objTransform.position;
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
                if (colliders[j].collider.CompareTag("Enemy") && hitDelay <= 0)
                {
                    ElementActiveDebuff debuff = colliders[j].collider.GetComponentInParent<ElementActiveDebuff>();
                    debuff.StartCoroutine(debuff.EffectTime(Elements.status.Grass, 5));

                    EnemyController.instance.TakeDamage(colliders[j].collider.GetComponent<EnemyState>(), basa.damage);
                    if (basa.stats[4].isTrigger)
                    {
                        if (colliders[j].collider.GetComponent<EnemyState>().health <= 0)
                        {
                            float heal = EnemyController.instance.enemies.First(s => s.prefab.mobName == colliders[j].collider.GetComponent<EnemyState>().mobName).healthMax * 0.1f;
                            if (DailyQuests.instance.quest.FirstOrDefault(s => s.id == 1 && s.isActive == true) != null)
                            {
                                DailyQuests.instance.UpdateValue(1, heal, false);
                            }
                            player.playerHealthPoint += heal;
                            player.fullFillImage.fillAmount = player.playerHealthPoint / player.playerHealthPointMax;
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
