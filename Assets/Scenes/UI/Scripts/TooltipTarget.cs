using UnityEngine;

public class TooltipTarget : MonoBehaviour
{
    public GameObject tooltipText;
    private bool isVisible = false;

    public void ToggleTooltip()
    {
        isVisible = !isVisible;
        tooltipText.SetActive(isVisible);
    }

    public void ShowTooltip()
    {
        isVisible = true;
        tooltipText.SetActive(true);
    }

    public void HideTooltip()
    {
        isVisible = false;
        tooltipText.SetActive(false);
    }
}
