using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fullFillImage;
    public SpriteRenderer buffArea;
    public float timeNeeded;
    public bool isNeedToMove;
    public void Start()
    {
    }
    public void Update()
    {
        if (!isNeedToMove)
        {
            MinusProgressBar();
        }
    }
    public void PlusProgressBar()
    {
        if (isNeedToMove)
        {
            fullFillImage.fillAmount += 1 / timeNeeded * Time.deltaTime;
        }
        if (fullFillImage.fillAmount == 1)
        {
            buffArea.color = new Color32(255, 240, 117, 87);
            isNeedToMove = false;
        }
    } 
    public void MinusProgressBar()
    {
        if (!isNeedToMove)
        {
            fullFillImage.fillAmount -= 1 / timeNeeded * Time.deltaTime;
        }
        if (fullFillImage.fillAmount == 0)
        {
            buffArea.color = new Color32(255, 255, 255, 87);
            isNeedToMove = true;
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            PlusProgressBar();
            if(buffArea.color != new Color32(255, 240, 117, 87))
            {
                isNeedToMove = true;
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        isNeedToMove = false;
    }
}
