using UnityEngine;
using UnityEngine.EventSystems;

public class CloseItem : MonoBehaviour, IPointerClickHandler
{
    public GameObject parent;
    public void OnPointerClick(PointerEventData eventData)
    {
        //GameManager.Instance.ClosePanel(GameManager.Instance.menuPanel);
        parent.GetComponentInChildren<MoveItem>().OnPointerClick(eventData);
    }
}
