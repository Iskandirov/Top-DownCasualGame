using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteVFX : MonoBehaviour
{
    public float stepDelete;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        //while (isTimerActive)
        {
            yield return new WaitForSeconds(stepDelete);
            Destroy(gameObject);
        }
    }
    
}
