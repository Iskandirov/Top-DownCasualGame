using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Rocket : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float angularSpeed = 720f; // градуси/сек для плавного повороту
    public bool faceVelocity = true; // чи обертати спрайт по напряму швидкості

    [Header("Homing (optional)")]
    public bool homing = false;
    public Transform homingTarget;
    public float homingStrength = 2f; // як сильно коригує напрямок (0 = не коригує)

    [Header("Lifetime")]
    public float lifetime = 8f; // автоматична деактивація через час

    Rigidbody2D rb;
    float lifeTimer;

    Vector2 moveDir = Vector2.left;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
    }

    private void OnEnable()
    {
        lifeTimer = 0f;
    }

    // Викликається спавнером, щоб налаштувати початкові параметри
    public void Launch(Vector2 direction, float speedOverride = -1f, Transform target = null)
    {
        moveDir = direction.normalized;
        if (speedOverride > 0f) speed = speedOverride;
        homingTarget = target;
        lifeTimer = 0f;

        rb.velocity = moveDir * speed;
        if (faceVelocity) RotateToVelocity();
    }

    private void FixedUpdate()
    {
        if (homing && homingTarget != null)
        {
            // коригування напрямку до цілі
            Vector2 toTarget = ((Vector2)homingTarget.position - rb.position).normalized;
            // інтерполюємо напрямок залежно від homingStrength
            Vector2 newDir = Vector2.Lerp(moveDir, toTarget, Time.fixedDeltaTime * homingStrength).normalized;
            moveDir = newDir;
            rb.velocity = moveDir * speed;
        }

        if (faceVelocity)
            RotateToVelocity();

        lifeTimer += Time.fixedDeltaTime;
        if (lifeTimer >= lifetime)
            gameObject.SetActive(false);
    }

    void RotateToVelocity()
    {
        Vector2 v = rb.velocity;
        if (v.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            // sprite default assumes right-facing (0 deg). Якщо ваш sprite спрямований вверх, додайте офсет.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), angularSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // тут ти можеш перевірити на зіткнення з гравцем або ландшафтом
        // приклад: якщо це гравець - викликати damage
        // деактивація при будь-якому контакті
        gameObject.SetActive(false);
    }
}