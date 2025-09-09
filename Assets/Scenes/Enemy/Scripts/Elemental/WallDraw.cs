using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDraw : MonoBehaviour
{
    public int textureSizeX = 512;
    public int textureSizeY = 512;
    public Color drawColor = Color.black;
    public Color WallColor = new Color(0, 0, 0, 0); // прозорий
    public float brushSize = 4f;

    private Texture2D drawTexture;
    private Renderer rend;
    private Vector2? lastUV = null;
    private bool needsApply = false;
    private int frameCounter = 0;
    private int applyFrameDelay = 2;

    void Start()
    {
        rend = GetComponent<Renderer>();

        drawTexture = new Texture2D(textureSizeX, textureSizeY, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;
        drawTexture.wrapMode = TextureWrapMode.Clamp;

        ClearTexture();

        rend.material.mainTexture = drawTexture;
    }

    void ClearTexture()
    {
        Color[] pixels = new Color[textureSizeX * textureSizeY];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = WallColor;

        drawTexture.SetPixels(pixels);
        drawTexture.Apply();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearTexture();
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 uv = hit.textureCoord;
                int x = (int)(uv.x * textureSizeX);
                int y = (int)(uv.y * textureSizeY);

                if (lastUV.HasValue)
                    DrawInterpolatedLine(lastUV.Value, new Vector2(x, y));
                else
                    DrawCircle(x, y);

                lastUV = new Vector2(x, y);
            }
        }
        else
        {
            lastUV = null;
        }

        if (needsApply && ++frameCounter >= applyFrameDelay)
        {
            drawTexture.Apply();
            frameCounter = 0;
            needsApply = false;
        }
    }

    void DrawCircle(int cx, int cy)
    {
        int r = Mathf.CeilToInt(brushSize);
        int rSquared = r * r;
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= rSquared)
                {
                    int px = cx + x;
                    int py = cy + y; // ← Без інверсії!
                    if (px >= 0 && px < textureSizeX && py >= 0 && py < textureSizeY)
                    {
                        drawTexture.SetPixel(px, py, drawColor);
                    }
                }
            }
        }
        needsApply = true;
    }

    void DrawInterpolatedLine(Vector2 from, Vector2 to)
    {
        float dist = Vector2.Distance(from, to);
        int steps = Mathf.CeilToInt(dist);
        for (int i = 0; i <= steps; i++)
        {
            Vector2 point = Vector2.Lerp(from, to, i / (float)steps);
            DrawCircle((int)point.x, (int)point.y);
        }
    }

    public Texture2D GetTexture() => drawTexture;
}
