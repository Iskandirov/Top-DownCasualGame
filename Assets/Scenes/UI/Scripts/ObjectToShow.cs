using UnityEngine;

public class ObjectToShow : MonoBehaviour
{
    public TooltipTarget tooltipTarget;
    bool isNeedToShow;
    private void Start()
    {
        tooltipTarget.HideTooltip();
        isNeedToShow = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            tooltipTarget.ShowTooltip();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isNeedToShow)
        {
            tooltipTarget.ShowTooltip();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tooltipTarget.HideTooltip();
            isNeedToShow = true;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isNeedToShow = false;
            gameObject.GetComponent<OjectToUpgrade>().Upgrade();
        }
    }
}