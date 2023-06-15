using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    public float timeToWin;
    public GameObject WinPanel;
    public GameObject WinPanelParent;
    public bool isShowed;
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(EndGame());
    }
    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(timeToWin);
        Instantiate(WinPanel, WinPanelParent.transform.position, Quaternion.identity, WinPanelParent.transform);

    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        text.text = time.ToString("00.00");
    }
}
