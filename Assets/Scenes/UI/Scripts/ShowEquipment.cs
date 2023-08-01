using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowEquipment : MonoBehaviour
{
    public Button button;
    public LoadEquipment equipPanel;
    public int count;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (equipPanel.gameObject.activeSelf)
        {
            equipPanel.gameObject.SetActive(false);
            equipPanel.DeleteList(equipPanel.list);
        }
        else
        {
            equipPanel.gameObject.SetActive(true);
            equipPanel.SetEquip(count);
        }
        equipPanel.transform.SetParent(transform);
    }
}
