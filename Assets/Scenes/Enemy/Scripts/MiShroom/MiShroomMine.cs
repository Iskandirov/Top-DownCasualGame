using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class MiShroomMine : MonoBehaviour
{
    public float damage;
    GameObject objectToHit;
    public MineSpawn parent;
    public bool inZone;
    PlayerManager player;
    public Animator anim;
    // Start is called before the first frame update
    private void Start()
    {
        player = PlayerManager.instance;   
        inZone = false;
        SetAlphaRecursively(transform, 0f);
    }
    private void SetAlphaRecursively(Transform parent, float alpha)
    {
        // �������� �� ��'���� SpriteRenderer � ������������ ��'���
        SpriteRenderer[] spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();

        // ������� ����� �� ������������ sorting order ��� ��� ��'���� SpriteRenderer
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha; // ������������ ����� � 1 ��� ����� ��������
            spriteRenderer.color = spriteColor;
        }
        if (GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>())
        {
            Color spriteColorLight = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().color;
            spriteColorLight.a = alpha; // ������������ ����� � 1 ��� ����� ��������
            GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().color = spriteColorLight;
        }
       
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield") && !collision.isTrigger)
        {
            objectToHit = collision.gameObject;
            inZone = true;
            SetAlphaRecursively(transform, 1f);
            anim.SetBool("Action", true);
        }
        else if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            objectToHit = collision.gameObject;
            inZone = true;
        }
        
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            inZone = false;
            objectToHit = null;
        }
        else if (collision.CompareTag("Shield") && !collision.isTrigger)
        {
            inZone = false;
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
