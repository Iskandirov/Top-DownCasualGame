using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [Header("References")]
    private PlayerManager player;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject landingVFXPrefab; // Префаб мітки
    [SerializeField] private Transform bossModel; // Модель боса для анімації (щоб відділити від логіки)

    [Header("Jump Settings")]
    [SerializeField] private float jumpUpHeight = 5f;          // Наскільки підлітає
    [SerializeField] private float jumpUpTime = 0.5f;          // Час підлітання
    [SerializeField] private float targetFollowDelay = 0.2f;   // Затримка руху мітки за гравцем
    [SerializeField] private float markerMoveTime = 1.5f;      // Скільки мітка рухається за гравцем
    [SerializeField] private float delayBeforeLanding = 0.5f;  // Пауза після зупинки мітки
    [SerializeField] private float landingSpeed = 20f;         // Швидкість падіння
    [SerializeField] private float damageRadius = 2f;          // Радіус ураження
    [SerializeField] private float damageAmount = 20f;         // Скільки шкоди наносить
    [SerializeField] private float knockbackForce = 5f;        // Відкидання гравця
    [SerializeField] private Vector2 damageEllipseSize = new Vector2(4f, 2f); // ширина і висота зони ураження
    [SerializeField] private Vector2 targetSize = new Vector2(4f, 2f); // ширина і висота зони ураження

    private bool isAttacking = false;
    private Vector2 lastLandingPos; // Запам'ятовуємо позицію для гізмос

    private void Start()
    {
        player = PlayerManager.instance;
    }

    public void ExecuteAttack()
    {
        if (!isAttacking)
            StartCoroutine(JumpAttackSequence());
    }

    private IEnumerator JumpAttackSequence()
    {
        isAttacking = true;

        Vector2 startPos = boss.transform.position;
        Vector2 upPos = startPos + Vector2.up * jumpUpHeight;

        // 1️⃣ Підлітання вгору
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / jumpUpTime;
            boss.transform.position = Vector2.Lerp(startPos, upPos, t);
            yield return null;
        }

        // Вирівнюємо X по гравцю
        Vector3 bossPos = boss.transform.position;
        bossPos.x = player.objTransform.position.x;
        boss.transform.position = bossPos;

        // 2️⃣ Створення VFX-мітки
        Vector2 visualSize = new Vector2(targetSize.x, targetSize.y); // ширина та висота еліпса у світі
        GameObject marker = Instantiate(landingVFXPrefab, player.objTransform.position, Quaternion.identity);
        Transform markerTransform = marker.transform;

        // Задаємо масштаб відносно світу
        markerTransform.localScale = new Vector3(
            visualSize.x / markerTransform.GetComponent<ParticleSystem>().shape.scale.x,
            visualSize.y / markerTransform.GetComponent<ParticleSystem>().shape.scale.y,
            1f
        );

        // Зберігаємо той самий розмір для зони ураження
        damageEllipseSize = visualSize;

        float elapsed = 0f;
        Vector2 targetPos = player.objTransform.position;

        // Мітка рухається за гравцем з затримкою
        while (elapsed < markerMoveTime)
        {
            targetPos = player.objTransform.position;
            markerTransform.position = Vector2.Lerp(markerTransform.position, targetPos, Time.deltaTime / targetFollowDelay);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3️⃣ Зупинка мітки
        yield return new WaitForSeconds(delayBeforeLanding);

        // 4️⃣ Приземлення
        Vector2 landingPos = markerTransform.position;

        Vector2 markerWorldSize = GetMarkerWorldSize(markerTransform);
        damageEllipseSize = markerWorldSize * 3; // тепер зона = реальному розміру VFX

        lastLandingPos = markerTransform.position;
        Destroy(marker);

        while (Vector2.Distance(boss.transform.position, landingPos) > 0.05f)
        {
            boss.transform.position = Vector2.MoveTowards(boss.transform.position, landingPos, landingSpeed * Time.deltaTime);
            yield return null;
        }

        // 5️⃣ Перевірка колізії (еліпс)

        Vector2 playerPos = player.objTransform.position;
        Vector2 ellipseCenter = boss.transform.position; // центр ураження
        float a = damageEllipseSize.x * 0.5f; // півширина
        float b = damageEllipseSize.y * 0.5f; // піввисота

        // Перетворення в локальні координати
        Vector2 local = playerPos - ellipseCenter;

        // Нормалізована відстань по еліпсу
        float ellipseCheck = (local.x * local.x) / (a * a) + (local.y * local.y) / (b * b);

        if (ellipseCheck <= 1f)
        {
            Debug.Log("Player hit by jump attack!");
            player.TakeDamage(damageAmount);

            Vector2 knockDir = (player.objTransform.position - boss.transform.position).normalized;
            player.rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
        }

        isAttacking = false;
    }
    private Vector2 GetMarkerWorldSize(Transform markerTransform)
    {
        ParticleSystem ps = markerTransform.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            // Основні налаштування
            var shape = ps.shape;

            // Базовий розмір форми (для Box / Ellipse)
            Vector3 shapeScale = shape.scale;

            // Конвертуємо в розміри світу (з урахуванням масштабу об'єкта)
            Vector2 worldSize = new Vector2(
                shapeScale.x * markerTransform.lossyScale.x,
                shapeScale.y * markerTransform.lossyScale.y
            );

            return worldSize;
        }

        // Фолбек — якщо нічого не знайшли
        return markerTransform.lossyScale;
    }
    // Gizmos для відображення радіуса шкоди
    private void OnDrawGizmos()
    {
        if (lastLandingPos == Vector2.zero)
            return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        DrawEllipseGizmo(lastLandingPos, damageEllipseSize, 40);
    }

    private void DrawEllipseGizmo(Vector2 center, Vector2 size, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Cos(rad) * size.x * 0.5f;
            float y = Mathf.Sin(rad) * size.y * 0.5f;
            Vector3 point = new Vector3(center.x + x, center.y + y, 0);

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }
    }
}