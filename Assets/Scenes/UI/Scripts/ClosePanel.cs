using UnityEngine;
using UnityEngine.UI;

public class ClosePanel : MonoBehaviour
{
    public Button button;

    private void Start()
    {
        // ������ �����-���������� ��� ��䳿 onClick ������
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        gameObject.SetActive(false);
    }

}
