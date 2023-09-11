using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fullFillImage;
    public bool isNeedToMove;
    public Vector2 startPos;
    public Health health;
    //public void PlusProgressBar(float plusHP)
    //{
    //    fullFillImage.localPosition = new Vector2(fullFillImage.transform.localPosition.x + plusHP / health.playerHealthPointMax * 80, fullFillImage.transform.localPosition.y);
    //}
    //public void MinusProgressBar(float minusHP)
    //{
    //    fullFillImage.localPosition = new Vector2(fullFillImage.transform.localPosition.x - minusHP / health.playerHealthPointMax * 80, fullFillImage.transform.localPosition.y);
    //}
    //public void MaxProgressBar()
    //{
    //    fullFillImage.localPosition = startPos;
    //}
}
