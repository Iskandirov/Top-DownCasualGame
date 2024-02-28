using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class MiShroomMine : MonoBehaviour
{
    public float damage;
    GameObject playerToHit;
    GameObject shieldToHit;
    public MineSpawn parent;
    PlayerManager player;
    public Animator anim;
    // Start is called before the first frame update
    private void Start()
    {
        player = PlayerManager.instance;   
        SetAlphaRecursively(transform, 0f);
    }
    private void SetAlphaRecursively(Transform parent, float alpha)
    {
        // Отримати всі компоненти SpriteRenderer у дочірніх об'єктах
        SpriteRenderer[] spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();

        // Перебрати всі SpriteRenderer та змінити їх прозорість
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha;
            spriteRenderer.color = spriteColor;
        }

        // Перевірити, чи є дочірні об'єкти з компонентом Light2D
        if (parent.GetComponentInChildren<Light2D>() != null)
        {
            // Отримати компонент Light2D у дочірньому об'єкті
            Light2D light2D = parent.GetComponentInChildren<Light2D>();

            // Змінити прозорість компонента Light2D
            Color lightColor = light2D.color;
            lightColor.a = alpha;
            light2D.color = lightColor;
        }

        // Рекурсивно викликати метод для дочірніх об'єктів
        foreach (Transform child in parent)
        {
            SetAlphaRecursively(child, alpha);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield"))
        {
            Debug.Log(4);
            shieldToHit = collision.gameObject;
            SetAlphaRecursively(transform, 1f);
            anim.SetBool("Action", true);
        }
        else if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (shieldToHit == null)
            {
                SetAlphaRecursively(transform, 1f);
                anim.SetBool("Action", true);
                playerToHit = collision.gameObject;
            }
        }

    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            playerToHit = null;
        }
        if (collision.CompareTag("Shield") && !collision.isTrigger)
        {
            playerToHit = null;
            shieldToHit = null;

        }
    }
    public void DamageDeal()
    {
        Debug.Log(1);
        if (shieldToHit != null)
        {
            Debug.Log(2);
            shieldToHit.GetComponent<Shield>().healthShield -= damage;
            GameManager.Instance.FindStatName("ShieldAbsorbedDamage", damage);
        }
        else if (playerToHit != null && !player.isInvincible)
        {
            Debug.Log(3);
            player.TakeDamage(damage);
        }
        parent.mineCount--;
        Destroy(gameObject);
    }
}
