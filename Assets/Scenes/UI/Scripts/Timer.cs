using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    public float timeToWin;
    public GameObject WinPanel;
    public GameObject WinPanelParent;
    public bool isShowed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        gameObject.GetComponent<TextMeshProUGUI>().text = time.ToString("00.00");
        if (time >= timeToWin && !isShowed)
        {
            Instantiate(WinPanel, WinPanelParent.transform.position, Quaternion.identity, WinPanelParent.transform);
            isShowed = true;
        }
    }
}
