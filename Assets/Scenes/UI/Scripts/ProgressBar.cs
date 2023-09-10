using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public Transform fullFillImage;
    public SpriteRenderer buffArea;
    public float timeNeeded;
    public bool isNeedToMove;
    public Vector2 startPos;
    public void Start()
    {
        startPos = fullFillImage.localPosition;
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
            fullFillImage.localPosition = new Vector2(fullFillImage.transform.localPosition.x + timeNeeded * Time.deltaTime, fullFillImage.transform.localPosition.y);
        }
        if (fullFillImage.localPosition.x >= startPos.x+80)
        {
            buffArea.color = new Color32(255, 240, 117, 87);
            isNeedToMove = false;
        }
    } 
    public void MinusProgressBar()
    {
        if (!isNeedToMove)
        {
            fullFillImage.localPosition = new Vector2(fullFillImage.transform.localPosition.x - timeNeeded * Time.deltaTime, fullFillImage.transform.localPosition.y);
        }
        if (fullFillImage.localPosition.x <= startPos.x)
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
