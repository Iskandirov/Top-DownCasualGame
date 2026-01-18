using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Trail : SkillBaseMono
{
    [Header("VFX and Visuals")]
    public VisualEffect vfx;
    public TrailRenderer trailRenderer;
    public LineRenderer leftBorder;
    public LineRenderer rightBorder;
    public Material mat;

    [Header("Settings")]
    public float size = 1f;               // ширина трейлу (відстань між межами)
    public float segmentLifetime = 1.5f;
    public float segmentMinDistance = 0.5f;

    [Header("Borders Visual Settings")]
    public float borderWidth = 0.06f;     // товщина лінії
    public Color borderColor = new Color(0f, 1f, 0.5f, 0.7f); // базовий колір
    public float fadeDuration = 2f;       // скільки секунд лінії зникають після появи

    private Vector3 lastPos;
    private List<TrailPoint> trailPoints = new List<TrailPoint>();

    private class TrailPoint
    {
        public Vector3 pos;
        public float timeAlive;
        public TrailPoint(Vector3 p) { pos = p; timeAlive = 0f; }
    }

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        player = PlayerManager.instance;
        lastPos = player.objTransform.position;
        lastPos.z = 0f;

        // --- Ініціалізація ліній ---
        if (leftBorder == null)
        {
            GameObject leftObj = new GameObject("LeftBorder");
            leftObj.transform.SetParent(transform);
            leftBorder = leftObj.AddComponent<LineRenderer>();
            SetupLineRenderer(leftBorder);
        }

        if (rightBorder == null)
        {
            GameObject rightObj = new GameObject("RightBorder");
            rightObj.transform.SetParent(transform);
            rightBorder = rightObj.AddComponent<LineRenderer>();
            SetupLineRenderer(rightBorder);
        }

        StartCoroutine(PeriodicSpawn());
    }

    private void SetupLineRenderer(LineRenderer lr)
    {
        lr.material = mat;
        lr.positionCount = 0;
        lr.startWidth = borderWidth;
        lr.endWidth = borderWidth;
        lr.useWorldSpace = true;
        lr.startColor = borderColor;
        lr.endColor = new Color(borderColor.r, borderColor.g, borderColor.b, 0f);
        lr.numCapVertices = 4;
    }

    private void FixedUpdate()
    {
        // --- Оновлення параметрів здібності ---
        if (basa.stats[1].isTrigger)
        {
            basa.lifeTime += basa.stats[1].value;
            trailRenderer.time = basa.lifeTime;
            basa.stats[1].isTrigger = false;
        }
        else if (basa.stats[2].isTrigger)
        {
            basa.damage += basa.stats[2].value;
            basa.stats[2].isTrigger = false;
        }
        else if (basa.stats[3].isTrigger)
        {
            size += basa.stats[3].value;
            basa.stats[3].isTrigger = false;
        }

        // --- Оновлення позиції у VFX ---
        vfx.SetVector3("PlayerPosition", player.objTransform.position);

        // --- Оновлення точок ---
        UpdateTrailPoints();
        UpdateTrailBorders();
    }

    IEnumerator PeriodicSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Vector3 currentPos = player.objTransform.position;
            currentPos.z = 0f;

            float distance = Vector3.Distance(currentPos, lastPos);
            if (distance > segmentMinDistance)
            {
                CreateColliderSegment(lastPos, currentPos);
                trailPoints.Add(new TrailPoint(currentPos)); // Додаємо точку!
                lastPos = currentPos;
            }
        }
    }

    List<Vector3> SmoothPath(List<Vector3> points, int smoothness = 10)
{
    List<Vector3> newPoints = new List<Vector3>();
    for (int i = 0; i < points.Count - 1; i++)
    {
        Vector3 p0 = points[Mathf.Max(i - 1, 0)];
        Vector3 p1 = points[i];
        Vector3 p2 = points[i + 1];
        Vector3 p3 = points[Mathf.Min(i + 2, points.Count - 1)];

        for (int j = 0; j < smoothness; j++)
        {
            float t = j / (float)smoothness;
            Vector3 newPoint = 0.5f * ((2 * p1) +
                (-p0 + p2) * t +
                (2*p0 - 5*p1 + 4*p2 - p3) * t * t +
                (-p0 + 3*p1 - 3*p2 + p3) * t * t * t);
            newPoints.Add(newPoint);
        }
    }
    return newPoints;
}

    void CreateColliderSegment(Vector3 start, Vector3 end)
    {
        start.z = 0f;
        end.z = 0f;
        Vector3 midPoint = (start + end) / 2f;
        midPoint.z = 0f;

        Vector2 dir = end - start;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject segment = new GameObject("TrailSegment");
        segment.transform.position = midPoint;
        segment.transform.rotation = Quaternion.Euler(0, 0, angle);
        segment.layer = LayerMask.NameToLayer("PlayerSpell");

        BoxCollider2D box = segment.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(distance, size);
        box.offset = Vector2.zero;

        TriggerTrail dealer = segment.AddComponent<TriggerTrail>();
        dealer.damage = basa.damage;
        dealer.damageInterval = 0.5f;
        dealer.basa = basa;

        Destroy(segment, trailRenderer.time);
    }

    private void UpdateTrailPoints()
    {
        // оновлюємо час життя кожної точки
        for (int i = 0; i < trailPoints.Count; i++)
        {
            trailPoints[i].timeAlive += Time.fixedDeltaTime;
        }

        // видаляємо старі точки, що віджили свій час
        trailPoints.RemoveAll(p => p.timeAlive > fadeDuration);
    }

    private void UpdateTrailBorders()
    {
        if (trailPoints.Count < 2) return;

        List<Vector3> rawPoints = trailPoints.Select(p => p.pos).ToList();
        List<Vector3> smoothPoints = SmoothPath(rawPoints, 10);

        int count = smoothPoints.Count;
        if (count < 2) return; // Додаємо додаткову перевірку

        Vector3[] left = new Vector3[count];
        Vector3[] right = new Vector3[count];
        Gradient gradient = new Gradient();

        // --- Обмежуємо кількість alpha keys до 8 ---
        int maxAlphaKeys = 8;
        GradientColorKey[] colorKeys = new GradientColorKey[1] { new GradientColorKey(borderColor, 0f) };
        GradientAlphaKey[] alphaKeys;

        if (count <= maxAlphaKeys)
        {
            alphaKeys = new GradientAlphaKey[count];
            for (int i = 0; i < count; i++)
            {
                float alpha = Mathf.InverseLerp(0, fadeDuration, fadeDuration - trailPoints[i % trailPoints.Count].timeAlive);
                alphaKeys[i] = new GradientAlphaKey(alpha, (float)i / count);
            }
        }
        else
        {
            alphaKeys = new GradientAlphaKey[maxAlphaKeys];
            for (int i = 0; i < maxAlphaKeys; i++)
            {
                int idx = Mathf.RoundToInt((float)i / (maxAlphaKeys - 1) * (count - 1));
                float alpha = Mathf.InverseLerp(0, fadeDuration, fadeDuration - trailPoints[idx % trailPoints.Count].timeAlive);
                alphaKeys[i] = new GradientAlphaKey(alpha, (float)i / maxAlphaKeys);
            }
        }
        gradient.SetKeys(colorKeys, alphaKeys);

        for (int i = 0; i < count; i++)
        {
            Vector3 forward;
            if (count == 2)
                forward = smoothPoints[1] - smoothPoints[0];
            else if (i == 0)
                forward = smoothPoints[1] - smoothPoints[0];
            else if (i == count - 1)
                forward = smoothPoints[count - 1] - smoothPoints[count - 2];
            else
                forward = smoothPoints[i + 1] - smoothPoints[i - 1];

            Vector3 side = Vector3.Cross(forward.normalized, Vector3.forward) * (size / 2f);
            left[i] = smoothPoints[i] - side;
            right[i] = smoothPoints[i] + side;
        }

        leftBorder.positionCount = count;
        rightBorder.positionCount = count;
        leftBorder.SetPositions(left);
        rightBorder.SetPositions(right);

        leftBorder.colorGradient = gradient;
        rightBorder.colorGradient = gradient;

        leftBorder.startWidth = borderWidth;
        leftBorder.endWidth = borderWidth;
        rightBorder.startWidth = borderWidth;
        rightBorder.endWidth = borderWidth;
    }
}