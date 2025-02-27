using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBehind : MonoBehaviour
{
    public List<GameObject> imagesToHover; // компонент дл€ зм≥ни прозорост≥
    public ParticleSystem particles; // компонент дл€ зм≥ни прозорост≥
    public int isSomeoneHere;

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (imagesToHover.Count > 0)
        {
            HideAndShow(collision, 1, isSomeoneHere + 1, 0.7f);
        }
        else
        {
            StartCoroutine(ReturnParticle(1f, 0f));
        }
    }

    [System.Obsolete]
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (imagesToHover.Count > 0)
        {
            HideAndShow(collision, 1, isSomeoneHere, 0.7f);
        }
        else
        {
            StartCoroutine(ReturnParticle(1f, 0f));
            //particles.startLifetime *= Time.fixedDeltaTime;
        }
    }

    [System.Obsolete]
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (imagesToHover.Count > 0)
        {
            HideAndShow(collision, 0, isSomeoneHere - 1, 1f);
        }
        else if(isActiveAndEnabled)
        {
            StartCoroutine(ReturnParticle(1f,1f));
        }
        
    }

    [System.Obsolete]
    public IEnumerator ReturnParticle(float delay, float targetCount)
    {
        yield return new WaitForSeconds(delay);

        // ѕерев≥римо targetCount перед циклом
        if (targetCount <= 0)
        {
            // якщо targetCount менше або дор≥внюЇ 0, встановимо startLifetime негайно
            particles.startLifetime = targetCount;
        }
        else
        {
            // ≤накше запустимо цикл дл€ зб≥льшенн€ startLifetime
            while (particles.startLifetime < targetCount)
            {
                particles.startLifetime = Mathf.Lerp(particles.startLifetime, targetCount, Time.deltaTime * 10);
                float difference = Mathf.Abs(particles.startLifetime - targetCount);

                if (difference <= 0.3f)
                {
                    break;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

    }
    void HideAndShow(Collider2D collision, int someOneHere, int operation, float opacity)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Enemy")) && !collision.isTrigger)
        {
            isSomeoneHere = operation;
            // якщо б≥льше немаЇ об'Їкт≥в всередин≥, зм≥нюЇмо прозор≥сть назад на повну
            if (isSomeoneHere == someOneHere)
            {
                foreach (var item in imagesToHover)
                {
                    if (item.GetComponent<SpriteRenderer>() != null)
                    {
                        Color c = item.GetComponent<SpriteRenderer>().color;
                        c.a = opacity;
                        item.GetComponent<SpriteRenderer>().color = c;
                    }
                    else 
                    {
                        Color c = item.GetComponent<Image>().color;
                        c.a = opacity;
                        item.GetComponent<Image>().color = c;
                    }


                }
            }
        }
    }
}
