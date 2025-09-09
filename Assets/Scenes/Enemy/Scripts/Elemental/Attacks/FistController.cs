using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistController : MonoBehaviour
{
    public enum FistState { Appear, Chase, Grab, Drag, Disappear }
    private FistState state;

    private Animator animator;
    private Transform bossTransform;
    private Transform player;
    private Transform playerStartPos;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private Transform hand;
    private Vector3 chaseTargetPos; // запам’ятана цільова позиція


    private float moveSpeed, grabRange, dragDuration, maxChaseTime;
    private bool hasGrabbed;

    private Quaternion initialRotation;

    public void Init(Transform boss, float moveSpeed, float maxChaseTime, float dragDuration, float grabRange)
    {
        this.bossTransform = boss;
        this.moveSpeed = moveSpeed;
        this.maxChaseTime = maxChaseTime;
        this.dragDuration = dragDuration;
        this.grabRange = grabRange;

        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        initialRotation = hand.rotation;

        StartCoroutine(FistRoutine());
    }
    private void Start()
    {
        playerStartPos = player; // зберігаємо початкову позицію гравця
    }
    private void Update()
    {
        if (state == FistState.Chase)
        {
            // напрямок до зафіксованої точки
            Vector3 direction = (chaseTargetPos - transform.position).normalized;

            // рух по прямій
            transform.position += direction * moveSpeed * Time.deltaTime;

            // поворот у бік польоту
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                hand.rotation = Quaternion.Slerp(hand.rotation, targetRot, Time.deltaTime * 10f);
            }

            // перевірка: чи перетнули гравця
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= grabRange)
            {
                hasGrabbed = true;
            }

            // якщо дісталися точки призначення і не схопили → зупинка
            if (Vector3.Distance(transform.position, chaseTargetPos) < 0.1f && !hasGrabbed)
            {
                state = FistState.Disappear;
                animator.SetTrigger("Disappear");
            }
        }
        else if (state == FistState.Drag)
        {
            // під час Drag тримаємо гравця на grabPoint
            player.position = grabPoint.position;

            // рухаємо руку назад до боса
            transform.position = Vector3.MoveTowards(
                transform.position,
                bossTransform.position,
                moveSpeed * Time.deltaTime
            );

            Vector3 dir = (bossTransform.position - transform.position).normalized;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                hand.rotation = Quaternion.Slerp(hand.rotation, targetRot, Time.deltaTime * 10f);
            }
        }
        else
        {
            // у всіх інших станах повертатись у початкову ротацію
            hand.rotation = Quaternion.Slerp(hand.rotation, initialRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator FistRoutine()
    {
        // --- APPEAR ---
        state = FistState.Appear;
        animator.SetTrigger("Appear");
        yield return WaitForAnimation("Appear");

        // --- CHASE ---
        state = FistState.Chase;
        animator.SetTrigger("Chase");

        // зафіксували точку, куди летітиме рука
        chaseTargetPos = player.position;

        float chaseTimer = 0f;
        hasGrabbed = false;

        while (chaseTimer < maxChaseTime && !hasGrabbed)
        {
            chaseTimer += Time.deltaTime;
            yield return null;
        }

        if (hasGrabbed)
        {
            // --- GRAB ---
            state = FistState.Grab;
            animator.SetTrigger("Grab");

            var playerManager = player.GetComponent<PlayerManager>();
            playerManager.enabled = false;


            yield return WaitForAnimation("Grab");

            // --- DRAG ---
            state = FistState.Drag;
            animator.SetTrigger("Drag");

            float elapsed = 0f;
            while (elapsed < dragDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Від’єднуємо
            playerManager.enabled = true;
        }

        // --- DISAPPEAR ---
        state = FistState.Disappear;
        animator.SetTrigger("Disappear");
        yield return WaitForAnimation("Disappear");

        Destroy(gameObject);
    }

    private IEnumerator WaitForAnimation(string stateName)
    {
        // чекаємо, поки анімація запуститься
        yield return null;
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        while (!info.IsName(stateName))
        {
            yield return null;
            info = animator.GetCurrentAnimatorStateInfo(0);
        }

        // чекаємо завершення анімації
        while (info.normalizedTime < 1f)
        {
            yield return null;
            info = animator.GetCurrentAnimatorStateInfo(0);
        }
    }
}
