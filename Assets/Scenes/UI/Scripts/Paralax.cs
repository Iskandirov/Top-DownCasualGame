using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    private float length, startPos;
    [SerializeField] GameObject mainCamera;
    public float parallaxEffect;
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float tempX = (mainCamera.transform.position.x * (1 - parallaxEffect));
        float distX = (mainCamera.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + distX,  transform.position.y, transform.position.z);
        if (tempX > startPos + length)
        {
            startPos += length;
        }
        else if(tempX < startPos - length)
        {
            startPos -= length;
        }
    }
}
