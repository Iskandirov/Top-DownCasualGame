using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    [SerializeField]TabGroup tabGroup;
    public Image bg;
    [SerializeField] UnityEvent onTabSelected;
    [SerializeField] UnityEvent onTabDeselected;
    public string tooltipText;
    public string statName;
    public string statValue;
    public string price;
    // Start is called before the first frame update
    void Start()
    {
        bg = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }
    public void Select()
    {
        if(onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
    }
    public void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this, tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
}
