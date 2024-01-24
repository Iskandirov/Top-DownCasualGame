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
            // �������� ����� ���������
            Vector2 colliderCenter = spawnMapBound.bounds.center;

            bool isCollided = false;
            do
            {
                // �������� ��������� ����� �������� ��������� �� ��������� Random.insideUnitCircle
                Vector2 randomPointInsideCollider = colliderCenter + Random.insideUnitCircle * (spawnMapBound.bounds.extents.magnitude);

                // ����������, �� ����� ����������� �������� ���
                if (randomPointInsideCollider.x >= spawnMapBound.bounds.min.x &&
                    randomPointInsideCollider.x <= spawnMapBound.bounds.max.x &&
                    randomPointInsideCollider.y >= spawnMapBound.bounds.min.y &&
                    randomPointInsideCollider.y <= spawnMapBound.bounds.max.y)
                {
                    // �������� ��'����, �� ������������� � ����� ������
                    Collider2D[] otherColliders = Physics2D.OverlapCircleAll(randomPointInsideCollider, 1f);

                    // ����������, �� � ����� ��� ��'��� � ����� �� �����, �� � �������� ��'���
                    foreach (Collider2D collider in otherColliders)
                    {
                        if (collider.gameObject.tag == zone.tag)
                        {
                            isCollided = true;
                            break;
                        }
                    }

                    // �������� ��'��� �� �������� �������
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
