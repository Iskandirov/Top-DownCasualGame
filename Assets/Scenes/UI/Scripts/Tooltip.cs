using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;
    [SerializeField] TextMeshProUGUI tooltipText;
    [SerializeField] RectTransform backgroundRectTransfrom;
    [SerializeField] RectTransform parentTransform;
    [SerializeField] Camera mainCamera;
    private void Awake()
    {
        instance = this;
        ShowTooltip("¬лад лох!");
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void SetToolTipText(string text)
    {
        tooltipText.text = text;
    }
    private void FixedUpdate()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, Input.mousePosition, mainCamera, out localPoint);
        transform.localPosition = localPoint;
    }
    void ShowTooltip(string toolTipString)
    {
        gameObject.SetActive(true);

        tooltipText.text = toolTipString;
        float textPaddingSize = 1f;

        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
        backgroundRectTransfrom.sizeDelta = backgroundSize;
    }
    void HideTooltip()
    {
        gameObject.SetActive(false);
    }
    public static void ShowTooltip_Static(string tooltipString)
    {
        instance.ShowTooltip(tooltipString);
    } 
    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
}
