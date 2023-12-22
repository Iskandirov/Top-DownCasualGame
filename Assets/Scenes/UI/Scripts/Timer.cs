using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    public GameObject WinPanel;
    public GameObject WinPanelParent;
    TextMeshProUGUI text;
    public bool isDropLooted;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(EndGame());
    }
    private IEnumerator EndGame()
    {
        // Корутина чекає на виконання умови
        while (!isDropLooted)
        {
            yield return null;
        }
        GameManager.Instance.OpenPanel(GameManager.Instance.winPanel);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        text.text = time.ToString("00.00");
    }
}
