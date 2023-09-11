using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class MiShroomMine : MonoBehaviour
{
    public float damage;
    Health player;
    public MineSpawn parent;
    public bool inZone;
    // Start is called before the first frame update
    private void Start()
    {
        inZone = false;
        player = FindObjectOfType<Health>();
        SetAlphaRecursively(transform, 0.5f);
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
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            inZone = true;
            SetAlphaRecursively(transform, 1f);
            GetComponent<Animator>().SetBool("Action",true);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            inZone = false;
        }
    }
    public void DamageDeal()
    {
        if (inZone)
        {
            player.playerHealthPoint -= damage;
            player.playerHealthPointImg.fullFillImage.fillAmount -= damage / player.playerHealthPointMax;
            player.GetComponent<Animator>().SetBool("IsHit", true);
        }
        parent.mineCount--;
        Destroy(gameObject);
    }
}
