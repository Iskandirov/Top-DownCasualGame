using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteExplodeToParticle : MonoBehaviour
{
    Texture2D sourceTexture;
    public SpriteRenderer sourceSprite;
    public GameObject pixelPrefab; // простий SpriteRenderer з квадратом 1x1
    public float pixelSize = 0.05f;
    public float explosionForce = 5f;
    public float lifetime = 2f;

    private void Start()
    {
        sourceTexture = sourceSprite.sprite.texture;
        StartCoroutine(SpawnPixels());
    }

    IEnumerator SpawnPixels()
    {
        for (int y = 0; y < sourceTexture.height; y++)
        {
            for (int x = 0; x < sourceTexture.width; x++)
            {
                Color color = sourceTexture.GetPixel(x, y);

                if (color.a < 0.1f)
                    continue;

                Vector3 spawnPos = transform.position + new Vector3(
                    (x - sourceTexture.width / 2f) * pixelSize,
                    (y - sourceTexture.height / 2f) * pixelSize,
                    0);

                GameObject pixel = Instantiate(pixelPrefab, spawnPos, Quaternion.identity, transform);
                pixel.transform.localScale = Vector3.one * pixelSize;
                pixel.GetComponent<SpriteRenderer>().color = color;

                // Apply random force
                Rigidbody2D rb = pixel.AddComponent<Rigidbody2D>();
                Vector2 dir = Random.insideUnitCircle.normalized;
                rb.AddForce(dir * explosionForce, ForceMode2D.Impulse);

                Destroy(pixel, lifetime);
            }

            yield return null; // дати Unity час, не зависати
        }

        // Optional: Destroy this object after
        Destroy(gameObject, lifetime + 0.5f);
    }
    private void OnDrawGizmos()
    {
        if (sourceTexture == null) return;

        float pixelSize = 0.1f;
        int width = sourceTexture.width;
        int height = sourceTexture.height;

        for (int y = 0; y < height; y += 2)
        {
            for (int x = 0; x < width; x += 2)
            {
                Color c = sourceTexture.GetPixel(x, y);
                if (c.a < 0.1f) continue;

                Vector3 pos = transform.position + new Vector3(
                    (x - width / 2f) * pixelSize,
                    (y - height / 2f) * pixelSize,
                    0
                );

                Gizmos.color = c;
                Gizmos.DrawSphere(pos, pixelSize * 0.5f);
            }
        }
    }
}
