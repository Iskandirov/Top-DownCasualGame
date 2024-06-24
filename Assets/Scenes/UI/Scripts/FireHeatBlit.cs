using Cyan;
using UnityEngine;
[System.Obsolete]
public class FireHeatBlit : MonoBehaviour
{
    [SerializeField] Blit blit;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = blit.settings.blitMaterial;
        mat.SetFloat("_VignetteIntensity", 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            mat.SetFloat("_VignetteIntensity", .71f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            mat.SetFloat("_VignetteIntensity", 0);
        }
    }
}
