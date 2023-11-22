using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeDialogFromAnim : MonoBehaviour
{
    TextAppear text;
    // Start is called before the first frame update
    void Start()
    {
        text = FindObjectOfType<TextAppear>();
    }

    public void CleanResume()
    {
        text.isPausedByAction = false;
        text.isPaused = false;
    }
    public void CleanPause()
    {
        text.isPausedByAction = true;
    }
}
