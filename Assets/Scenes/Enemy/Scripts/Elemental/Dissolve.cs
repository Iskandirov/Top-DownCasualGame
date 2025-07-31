using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float dissolveTime = .75f;

    private SpriteRenderer[] spriteRenderers;
    private Material[] dissolveMaterial;

    private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public bool isDissolving = false;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        dissolveMaterial = new Material[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            dissolveMaterial[i] = spriteRenderers[i].material;
        }
        StartCoroutine(Vanish());
    }

    public void AppearOrVanish()
    {
        if (isDissolving)
        {
            StartCoroutine(Vanish());
        }
        else
        {
            StartCoroutine(Appear());
        }
    }
    private IEnumerator Vanish()
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / dissolveTime));
            for (int i = 0; i < dissolveMaterial.Length; i++)
            {
                dissolveMaterial[i].SetFloat(dissolveAmount, lerpedDissolve);
            }
            yield return null;
        }
    } 
    private IEnumerator Appear()
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(1.1f, 0, (elapsedTime / dissolveTime));
            for (int i = 0; i < dissolveMaterial.Length; i++)
            {
                dissolveMaterial[i].SetFloat(dissolveAmount, lerpedDissolve);
            }
            yield return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            StartCoroutine(Appear());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            StartCoroutine(Vanish());
        }
    }
}

