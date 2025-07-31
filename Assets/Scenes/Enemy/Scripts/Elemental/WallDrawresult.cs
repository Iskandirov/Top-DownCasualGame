using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDrawresult : MonoBehaviour
{
    public GameObject wallResultPrefab;
    public GameObject plateResultPrefab;
    public SpriteRenderer resultSpriteObj;
    public List<Sprite> resultVariantSprite;
    public List<Color> resultVariantColor;
    public float moveSpeed = 1f; // Швидкість руху
    public Vector3 newPoint;

    public void SetResult(bool success)
    {
        resultSpriteObj.sprite = success ? resultVariantSprite[0] : resultVariantSprite[1];
        resultSpriteObj.color = success ? resultVariantColor[0] : resultVariantColor[1];

        StartCoroutine(MoveToPosition()); // Запускаємо плавний рух
    }

    private IEnumerator MoveToPosition()
    {
        Vector3 worldTarget = wallResultPrefab.transform.parent.TransformPoint(newPoint);

        while (Vector3.Distance(wallResultPrefab.transform.position, worldTarget) > 0.01f)
        {
            wallResultPrefab.transform.position = Vector3.MoveTowards(
                wallResultPrefab.transform.position,
                worldTarget,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
        plateResultPrefab.SetActive(true);

        wallResultPrefab.transform.position = worldTarget;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPoint, 0.1f);
    }
}
