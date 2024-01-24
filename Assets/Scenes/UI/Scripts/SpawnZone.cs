using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    public ProgressBar zone;
    public Collider2D spawnMapBound;
    public List<Sprite> spritesBuff;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < zone.buffTypes.Count; i++)
        {
            // Отримуємо центр колайдера
            Vector2 colliderCenter = spawnMapBound.bounds.center;

            bool isCollided = false;
            do
            {
                // Отримуємо випадкову точку всередині колайдера за допомогою Random.insideUnitCircle
                Vector2 randomPointInsideCollider = colliderCenter + Random.insideUnitCircle * (spawnMapBound.bounds.extents.magnitude);

                // Перевіряємо, чи точка знаходиться всередині меж
                if (randomPointInsideCollider.x >= spawnMapBound.bounds.min.x &&
                    randomPointInsideCollider.x <= spawnMapBound.bounds.max.x &&
                    randomPointInsideCollider.y >= spawnMapBound.bounds.min.y &&
                    randomPointInsideCollider.y <= spawnMapBound.bounds.max.y)
                {
                    // Отримуємо об'єкти, які перетинаються з даною точкою
                    Collider2D[] otherColliders = Physics2D.OverlapCircleAll(randomPointInsideCollider, 1f);

                    // Перевіряємо, чи є серед них об'єкт з таким же тегом, як і поточний об'єкт
                    foreach (Collider2D collider in otherColliders)
                    {
                        if (collider.gameObject.tag == zone.tag)
                        {
                            isCollided = true;
                            break;
                        }
                    }

                    // Спавнуємо об'єкт на отриманій позиції
                    if (!isCollided)
                    {
                        ProgressBar bar = Instantiate(zone, randomPointInsideCollider, Quaternion.identity, transform);
                        bar.buffTypes[i] = true;
                        if (bar.GetComponentsInChildren<SpriteRenderer>().Any(s => s.gameObject.tag == "EditorOnly"))
                        {
                            bar.GetComponentsInChildren<SpriteRenderer>()
                            .Where(s => s.gameObject.tag == "EditorOnly")
                            .Select(s => s.sprite = spritesBuff[i])
                            .ToList();
                        }

                    }
                }
            } while (isCollided);
        }
    }
}
