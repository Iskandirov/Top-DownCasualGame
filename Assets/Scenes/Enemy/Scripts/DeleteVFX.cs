using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteVFX : MonoBehaviour
{
    public float stepDelete;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stepDelete <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            stepDelete -= Time.deltaTime;
        }
    }
}
