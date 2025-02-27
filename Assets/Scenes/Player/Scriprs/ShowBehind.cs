using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBehind : MonoBehaviour
{
    public List<GameObject> imagesToHover; // ��������� ��� ���� ���������
    public ParticleSystem particles; // ��������� ��� ���� ���������
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

        // ��������� targetCount ����� ������
        if (targetCount <= 0)
        {
            // ���� targetCount ����� ��� ������� 0, ���������� startLifetime �������
            particles.startLifetime = targetCount;
        }
        else
        {
            // ������ ��������� ���� ��� ��������� startLifetime
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
            // ���� ����� ���� ��'���� ��������, ������� ��������� ����� �� �����
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
