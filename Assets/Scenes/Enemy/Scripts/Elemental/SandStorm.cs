using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;

public class SandStorm : MonoBehaviour
{
    float duration;
    float damagePerSecond;
    float slowMultiplier;
    [SerializeField] float followSpeed;

    PlayerManager player;
    bool isSlowed = false;
    public bool inZone = false;
    public void Setup(float d, float damage, float slow)
    {
        player = PlayerManager.instance;
        duration = d;
        damagePerSecond = damage;
        slowMultiplier = slow;
        StartCoroutine(ApplyEffects());
    }

    private IEnumerator ApplyEffects()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (inZone)
            {
                if (!isSlowed)
                {
                    player.StartSlowPlayer(duration, slowMultiplier);
                    isSlowed = true;
                }
                player.TakeDamage(damagePerSecond * Time.deltaTime);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
   
    private void FixedUpdate()
    {
        if (player != null)
        {
            // Перемещаем SandStorm в направлении игрока с заданной скоростью
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, followSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            inZone = true;
            ApplyEffects();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            inZone = false;
            isSlowed = false;
        }
    }
}
