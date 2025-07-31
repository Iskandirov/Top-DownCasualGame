using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[ExecuteAlways]
public class DrawComparer : MonoBehaviour
{
    public WallDraw painter;
    public SpriteRenderer referenceTexture;
    [Range(0f, 1f)] public float alphaThreshold = 0.1f;
    public float debugScale = 1f;
    public float matchRadius = 0.01f; // у world-space

    private struct DebugPixel
    {
        public int x, y;
        public Color color;
        public DebugPixel(int x, int y, Color color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }
    }

    private List<DebugPixel> debugPixels = new List<DebugPixel>();

    [ContextMenu("Compare Optimized Accurate")]
    public float Compare()
    {
        if (painter == null || referenceTexture == null || referenceTexture.sprite == null)
            return 0f;

        Texture2D playerRaw = painter.GetTexture();
        Texture2D referenceRaw = ExtractSpriteTexture(referenceTexture.sprite);

        debugPixels.Clear();

        float threshold = alphaThreshold;
        float radiusSqr = matchRadius * matchRadius;

        // Bounds
        Bounds playerBounds = painter.GetComponent<Renderer>().bounds;
        Bounds refBounds = referenceTexture.GetComponent<Renderer>().bounds;

        Vector2 playerOrigin = playerBounds.min;
        Vector2 playerSize = playerBounds.size;
        Vector2 refOrigin = refBounds.min;
        Vector2 refSize = refBounds.size;

        int pWidth = playerRaw.width;
        int pHeight = playerRaw.height;
        int rWidth = referenceRaw.width;
        int rHeight = referenceRaw.height;

        float pPixelW = playerSize.x / pWidth;
        float pPixelH = playerSize.y / pHeight;
        float rPixelW = refSize.x / rWidth;
        float rPixelH = refSize.y / rHeight;

        // Побудова spatial index для reference
        Dictionary<Vector2Int, List<Vector2>> refGrid = new();
        float cellSize = matchRadius;

        for (int x = 0; x < rWidth; x++)
        {
            for (int y = 0; y < rHeight; y++)
            {
                if (referenceRaw.GetPixel(x, y).a > threshold)
                {
                    Vector2 worldPos = refOrigin + new Vector2((x + 0.5f) * rPixelW, (y + 0.5f) * rPixelH);
                    Vector2Int cell = GetCell(worldPos, cellSize);
                    if (!refGrid.ContainsKey(cell))
                        refGrid[cell] = new List<Vector2>();
                    refGrid[cell].Add(worldPos);
                }
            }
        }

        int intersection = 0;
        int union = 0;

        // Перевірка пікселів гравця
        for (int x = 0; x < pWidth; x++)
        {
            for (int y = 0; y < pHeight; y++)
            {
                if (playerRaw.GetPixel(x, y).a <= threshold)
                    continue;

                Vector2 pWorld = playerOrigin + new Vector2((x + 0.5f) * pPixelW, (y + 0.5f) * pPixelH);
                bool matched = false;

                Vector2Int cell = GetCell(pWorld, cellSize);
                for (int dx = -1; dx <= 1 && !matched; dx++)
                {
                    for (int dy = -1; dy <= 1 && !matched; dy++)
                    {
                        Vector2Int neighbor = new Vector2Int(cell.x + dx, cell.y + dy);
                        if (refGrid.TryGetValue(neighbor, out var refPoints))
                        {
                            foreach (var r in refPoints)
                            {
                                if ((r - pWorld).sqrMagnitude <= radiusSqr)
                                {
                                    matched = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (matched)
                {
                    intersection++;
                }
                else
                {
                    debugPixels.Add(new DebugPixel(x, y, Color.red));
                }

                union++;
            }
        }

        // Пікселі еталону, яких немає в гравці
        for (int x = 0; x < rWidth; x++)
        {
            for (int y = 0; y < rHeight; y++)
            {
                if (referenceRaw.GetPixel(x, y).a <= threshold)
                    continue;

                Vector2 rWorld = refOrigin + new Vector2((x + 0.5f) * rPixelW, (y + 0.5f) * rPixelH);

                Vector2Int cell = GetCell(rWorld, cellSize);
                bool matched = false;

                for (int dx = -1; dx <= 1 && !matched; dx++)
                {
                    for (int dy = -1; dy <= 1 && !matched; dy++)
                    {
                        Vector2 checkPos = rWorld + new Vector2(dx * cellSize, dy * cellSize);
                        Vector2Int pCell = GetCell(checkPos, cellSize);
                        if (refGrid.TryGetValue(pCell, out var dummy)) continue; // avoid double-check

                        // Reuse previous method: brute force check on player texture
                        int px = Mathf.FloorToInt((rWorld.x - playerOrigin.x) / pPixelW);
                        int py = Mathf.FloorToInt((rWorld.y - playerOrigin.y) / pPixelH);

                        if (px >= 0 && py >= 0 && px < pWidth && py < pHeight)
                        {
                            if (playerRaw.GetPixel(px, py).a > threshold)
                            {
                                matched = true;
                                break;
                            }
                        }
                    }
                }

                if (!matched)
                {
                    union++;

                    int dx = Mathf.FloorToInt((rWorld.x - playerOrigin.x) / pPixelW);
                    int dy = Mathf.FloorToInt((rWorld.y - playerOrigin.y) / pPixelH);
                    debugPixels.Add(new DebugPixel(dx, dy, Color.blue));
                }
            }
        }

        float similarity = union == 0 ? 0f : (float)intersection / union;
        Debug.Log($"Similarity: {(similarity * 100f):F2}%");
        return similarity;
    }

    private Vector2Int GetCell(Vector2 pos, float cellSize)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / cellSize),
            Mathf.FloorToInt(pos.y / cellSize)
        );
    }

    private Texture2D ExtractSpriteTexture(Sprite sprite)
    {
        Rect rect = sprite.textureRect;
        Texture2D fullTex = sprite.texture;
        Texture2D cropped = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        Color[] pixels = fullTex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        cropped.SetPixels(pixels);
        cropped.Apply();
        return cropped;
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        if (painter == null || debugPixels == null || debugPixels.Count == 0) return;

//        Vector3 origin = painter.transform.position;
//        Texture2D tex = painter.GetTexture();
//        if (tex == null) return;

//        float texWidth = tex.width;
//        float texHeight = tex.height;

//        Vector3 sizeWorld = painter.GetComponent<Renderer>().bounds.size;
//        float pixelSizeX = sizeWorld.x / texWidth;
//        float pixelSizeY = sizeWorld.y / texHeight;

//        foreach (var px in debugPixels)
//        {
//            Vector3 worldPos = origin - sizeWorld / 2f + new Vector3(
//                (px.x + 0.5f) * pixelSizeX,
//                (px.y + 0.5f) * pixelSizeY,
//                0f
//            );

//            Gizmos.color = px.color;
//            Gizmos.DrawCube(worldPos, new Vector3(pixelSizeX, pixelSizeY, 0.01f));
//        }
//    }
//#endif
}
