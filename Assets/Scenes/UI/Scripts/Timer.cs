using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    public float timeToWin;
    public GameObject WinPanel;
    public GameObject WinPanelParent;
    TextMeshProUGUI text;
    Move PanelChecker;
    public bool isDropLooted;
    // Start is called before the first frame update
    void Start()
    {
        PanelChecker = FindObjectOfType<Move>();
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(EndGame());
    }
    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(timeToWin);

        // Корутина чекає на виконання умови
        while (!isDropLooted)
        {
            yield return null;
        }

        Instantiate(WinPanel, WinPanelParent.transform.position, Quaternion.identity, WinPanelParent.transform);
        PanelChecker.otherPanelOpened = true;
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        text.text = time.ToString("00.00");
    }
}
