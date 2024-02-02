using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MineSpawn : MonoBehaviour
{
    public int mineCount;
    public MiShroomMine mine;
    public float delay;
    float delayMax;
    public float radius;
    public bool inZone;
    public SpriteRenderer shadow;
    public ShadowCaster2D shadow2D;
    public Light2D light2D;
    Transform objTransform;

    // Start is called before the first frame update
    void Start()
    {
        objTransform = transform;
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
                shadow2D.enabled = true;
                Color spriteColorIgnore = spriteToIgnore.color;
                spriteColorIgnore.a = 0.2f;
                spriteToIgnore.color = spriteColorIgnore;
            }
            else
            {
                shadow2D.enabled = false;
            }
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha; // ������������ ����� � 1 ��� ����� ��������
            spriteRenderer.color = spriteColor;

            Color spriteColorLight = light2D.color;
            spriteColorLight.a = alpha; // ������������ ����� � 1 ��� ����� ��������
            light2D.color = spriteColorLight;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (mineCount < 10)
        {
            delay -= Time.fixedDeltaTime;
        }
        if (mineCount < 10 && delay <= 0)
        {
            float initialZ = transform.position.z; // �������� ��������� �������� Z

            MiShroomMine a = Instantiate(mine, new Vector3(objTransform.position.x, objTransform.position.y, initialZ) + Random.insideUnitSphere * radius, Quaternion.identity, objTransform);
            Transform aTrans = a.transform;
            // ϳ��� ��������� ��'���� ���������� ��������� �������� Z
            aTrans.position = new Vector3(aTrans.position.x, aTrans.position.y, initialZ);
            delay = delayMax;
            mineCount++;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            inZone = true;
            SetAlphaRecursively(objTransform, 1f, shadow);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            inZone = false;
            SetAlphaRecursively(objTransform, 0f);
        }
    }
}
