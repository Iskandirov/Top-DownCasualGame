using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : MonoBehaviour
{
    public float destroyDelay = 1f;

    public float speed = 1f; // �������� ����

    private void Start()
    {
    }

    public void CreateMessage(string message)
    {
        // ��������� ��'��� ������ �� ������ ���� ���������
        GameObject popUpText = new GameObject("PopUpText");
        popUpText.transform.SetParent(transform, false);

        TextMeshProUGUI textMesh = popUpText.AddComponent<TextMeshProUGUI>();
        Rigidbody2D popUpTextRigid = popUpText.AddComponent<Rigidbody2D>();
        popUpTextRigid.gravityScale = -0.5f;
        textMesh.text = message;
        textMesh.fontSize = 24;
        textMesh.color = Color.white;

        // ��������� �������� ��� ��������� ������ ����� ������� ������
        StartCoroutine(DestroyPopUpText(popUpText.gameObject));
    }
    private IEnumerator DestroyPopUpText(GameObject popUpText)
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(popUpText);
    }
}