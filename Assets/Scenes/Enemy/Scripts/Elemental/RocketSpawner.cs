using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    public enum SpawnEdge { Left, Right, Top, Bottom, Any }

    [Header("Prefab & Pool")]
    public GameObject rocketPrefab;
    public int poolSize = 20;
    public GameObject warningPrefab;

    [Header("Spawn Settings")]
    public SpawnEdge edge = SpawnEdge.Any;
    public float spawnInterval = 1.2f;
    public float spawnIntervalRandom = 0.6f;
    public float spawnOffset = 1f;
    public Vector2 speedRange = new Vector2(4f, 9f);
    public float warningTime = 1.5f;
    public Vector2 topOffset = new Vector2(0, 0);
    public Vector2 bottomOffset = new Vector2(0, 0);
    public Vector2 leftOffset = new Vector2(0, 0);
    public Vector2 rightOffset = new Vector2(0, 0);
    public float smoothness = .5f;

    [Header("Behavior")]
    public bool allowHoming = false;
    public Transform homingTarget;
    public float homingChance = 0.25f;
    public float homingStrength = 2f;
    public float rocketLifetime = 8f;

    [Header("Misc")]
    public Camera referenceCamera;
    public DragAndDropTrap obj;
    public bool spawnOnStart = true;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private float spawnTimer;

    private void Awake()
    {
        if (referenceCamera == null) referenceCamera = Camera.main;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(rocketPrefab, transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    private void Start()
    {
        if (spawnOnStart) ScheduleNextSpawn(0f);
    }

    void ScheduleNextSpawn(float minDelay)
    {
        float baseDelay = Mathf.Max(0.01f, spawnInterval);
        float r = Random.Range(-spawnIntervalRandom, spawnIntervalRandom);
        spawnTimer = baseDelay + r;
        if (minDelay > 0f) spawnTimer = Mathf.Min(spawnTimer, minDelay);
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && obj.IsBeingDragged)
        {
            StartCoroutine(SpawnRocketWithWarning());
            ScheduleNextSpawn(0f);
        }
    }

    IEnumerator SpawnRocketWithWarning()
    {
        SpawnEdge actualEdge = edge;
        if (edge == SpawnEdge.Any) actualEdge = (SpawnEdge)Random.Range(0, 4);

        Vector2 viewportPos = GetViewportSpawnPos(actualEdge);
        switch (actualEdge)
        {
            case SpawnEdge.Top: viewportPos += topOffset; break;
            case SpawnEdge.Bottom: viewportPos += bottomOffset; break;
            case SpawnEdge.Left: viewportPos += leftOffset; break;
            case SpawnEdge.Right: viewportPos += rightOffset; break;
        }
        Vector2 dir = GetDirectionFromEdge(actualEdge);

        GameObject warning = null;
        if (warningPrefab != null)
        {
            warning = Instantiate(warningPrefab);
            warning.GetComponent<TextMeshPro>().text = actualEdge.ToString();

            // Після Instantiate(warningPrefab)
            warning.transform.SetParent(referenceCamera.transform, false);

            // Встановлюємо локальну позицію на краю вікна
            Vector3 localPos = referenceCamera.ViewportToWorldPoint(
                new Vector3(viewportPos.x, viewportPos.y, referenceCamera.nearClipPlane + 1f)
            );
            localPos = referenceCamera.transform.InverseTransformPoint(localPos);
            warning.transform.localPosition = localPos;

            StartCoroutine(UpdateWarningPosition(warning.transform, viewportPos));
        }

        yield return new WaitForSeconds(warningTime);

        if (warning != null)
        {
            Vector3 spawnWorldPos = referenceCamera.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, referenceCamera.nearClipPlane));
            Destroy(warning);
            SpawnRocket(spawnWorldPos, dir);
        }
    }

    IEnumerator UpdateWarningPosition(Transform warningTransform, Vector2 viewportPos)
    {
        float fixedZ = 0f;
        while (warningTransform != null)
        {
            Vector3 localPos = referenceCamera.ViewportToWorldPoint(
                new Vector3(viewportPos.x, viewportPos.y, referenceCamera.nearClipPlane + 1f)
            );
            localPos = referenceCamera.transform.InverseTransformPoint(localPos);
            warningTransform.localPosition = localPos;
            yield return null;
        }
    }

    void SpawnRocket(Vector2 pos, Vector2 dir)
    {
        if (pool.Count == 0) return;
        GameObject go = pool.Dequeue();
        go.SetActive(true);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.identity;

        Rocket rocket = go.GetComponent<Rocket>();
        if (rocket == null) rocket = go.AddComponent<Rocket>();

        float spd = Random.Range(speedRange.x, speedRange.y);
        rocket.speed = spd;
        rocket.lifetime = rocketLifetime;
        rocket.homing = false;
        rocket.homingTarget = null;
        rocket.homingStrength = homingStrength;

        if (allowHoming && homingTarget != null && Random.value < homingChance)
        {
            rocket.homing = true;
            rocket.homingTarget = homingTarget;
        }

        rocket.Launch(dir, spd, rocket.homingTarget);

        StartCoroutine(ReturnToPoolWhenDisabled(go));
    }

    IEnumerator ReturnToPoolWhenDisabled(GameObject go)
    {
        while (go.activeInHierarchy) yield return null;

        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        pool.Enqueue(go);
    }

    Vector2 GetViewportSpawnPos(SpawnEdge e)
    {
        float t = Random.value;
        switch (e)
        {
            case SpawnEdge.Left: return new Vector2(0f, t);
            case SpawnEdge.Right: return new Vector2(1f, t);
            case SpawnEdge.Top: return new Vector2(t, 1f);
            case SpawnEdge.Bottom: return new Vector2(t, 0f);
        }
        return new Vector2(0f, t);
    }

    Vector2 GetDirectionFromEdge(SpawnEdge e)
    {
        switch (e)
        {
            case SpawnEdge.Left: return Vector2.right;
            case SpawnEdge.Right: return Vector2.left;
            case SpawnEdge.Top: return Vector2.down;
            case SpawnEdge.Bottom: return Vector2.up;
        }
        return Vector2.left;
    }
}