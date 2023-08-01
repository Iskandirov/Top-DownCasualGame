using UnityEngine;
using UnityEngine.UI;

public class ClosePanel : MonoBehaviour
{
    public Button button;

    private void Start()
    {
        // Додаємо метод-обработчик для події onClick кнопки
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        gameObject.SetActive(false);
    }

}
