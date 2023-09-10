using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawn : MonoBehaviour
{
    public int mineCount;
    public MiShroomMine mine;
    public float delay;
    float delayMax;
    public float radius;
    public bool inZone;
    public SpriteRenderer shadow;

    // Start is called before the first frame update
    void Start()
    {
        delayMax = delay;
        SetAlphaRecursively(transform, 0f);
    }
    private void SetAlphaRecursively(Transform parent, float alpha, SpriteRenderer spriteToIgnore = null)
    {
        // �������� �� ��'���� SpriteRenderer � ������������ ��'���
        SpriteRenderer[] spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();

        // ������� ����� �� ������������ sorting order ��� ��� ��'���� SpriteRenderer
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer == spriteToIgnore)
                continue; // ���������� ������� ������
            if (alpha == 1)
            {
                Color spriteColorIgnore = spriteToIgnore.color;
                spriteColorIgnore.a = 0.2f;
                spriteToIgnore.color = spriteColorIgnore;
            }
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha; // ������������ ����� � 1 ��� ����� ��������
            spriteRenderer.color = spriteColor;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (mineCount < 20)
        {
            delay -= Time.deltaTime;
        }
        if (mineCount < 20 && delay <= 0)
        {
            float initialZ = transform.position.z; // �������� ��������� �������� Z

            MiShroomMine a = Instantiate(mine, new Vector3(transform.position.x, transform.position.y, initialZ) + Random.insideUnitSphere * radius, Quaternion.identity);

            // ϳ��� ��������� ��'���� ���������� ��������� �������� Z
            a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y, initialZ);
            a.parent = this;
            delay = delayMax;
            mineCount++;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            inZone = true;
            SetAlphaRecursively(transform, 1f, shadow);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            inZone = false;
            SetAlphaRecursively(transform, 0f);
        }
    }
}
