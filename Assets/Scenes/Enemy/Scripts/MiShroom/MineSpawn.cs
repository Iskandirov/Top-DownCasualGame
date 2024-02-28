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
    public ShadowCaster2D shadow2D;
    public Light2D light2D;
    Transform objTransform;

    // Start is called before the first frame update
    void Start()
    {
        mineCount = 0;
        objTransform = transform;
        delayMax = delay;
        SetAlphaRecursively(transform, 0f);
    }
    private void SetAlphaRecursively(Transform parent, float alpha)
    {
        SpriteRenderer[] spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            shadow2D.enabled = alpha == 1 ? true : false;

            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha; 
            spriteRenderer.color = spriteColor;

            Color spriteColorLight = light2D.color;
            spriteColorLight.a = alpha; 
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
            Transform initial = transform; // Зберегти початкове значення Z

            MiShroomMine a = Instantiate(mine, new Vector3(initial.position.x, initial.position.y, initial.position.z) + Random.insideUnitSphere * radius, Quaternion.identity,transform.parent);
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
            SetAlphaRecursively(objTransform, 1f);
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
