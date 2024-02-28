using UnityEngine;
using UnityEngine.EventSystems;

public class CloseItem : MonoBehaviour, IPointerClickHandler
{
    public GameObject parent;
    public void OnPointerClick(PointerEventData eventData)
    {
        parent.GetComponentInChildren<MoveItem>().OnPointerClick(eventData);
    }
}
