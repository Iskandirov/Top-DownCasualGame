using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MiShroomMine : MonoBehaviour
{
    public float damage;
    GameObject objectToHit;
    public MineSpawn parent;
    public bool inZone;
    PlayerManager player;
    // Start is called before the first frame update
    private void Start()
    {
        player = PlayerManager.instance;   
        inZone = false;
        SetAlphaRecursively(transform, 0f);
    }
    private void SetAlphaRecursively(Transform parent, float alpha)
    {
        // Отримуємо всі об'єкти SpriteRenderer у батьківському об'єкті
        SpriteRenderer[] spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();

        // Змінюємо альфа та встановлюємо sorting order для всіх об'єктів SpriteRenderer
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha; // Встановлюємо альфу в 1 для повної видимості
            spriteRenderer.color = spriteColor;
        }
        if (GetComponentInChildren<Light2D>())
        {
            Color spriteColorLight = GetComponentInChildren<Light2D>().color;
            spriteColorLight.a = alpha; // Встановлюємо альфу в 1 для повної видимості
            GetComponentInChildren<Light2D>().color = spriteColorLight;
        }
       
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && player.gameObject != null)
        {
            objectToHit = player.gameObject;
            inZone = true;
            SetAlphaRecursively(transform, 1f);
            FindObjectOfType<AudioSource>().Play();
            GetComponent<Animator>().SetBool("Action", true);
        }
        else if (collision.CompareTag("Shield"))
        {
            objectToHit = collision.gameObject;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            inZone = false;
        }
        else if (collision.CompareTag("Shield"))
        {
            objectToHit = null;
        }
    }
    public void DamageDeal()
    {
        if (inZone && objectToHit.CompareTag("Shield"))
        {
            objectToHit.GetComponent<Shield>().healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
        }
        else if (inZone && objectToHit.CompareTag("Player") && !player.isInvincible)
        {
            player.TakeDamage(damage);
        }
        parent.mineCount--;
        Destroy(gameObject);
    }
}
