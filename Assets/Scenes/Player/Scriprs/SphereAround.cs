using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAround : MonoBehaviour
{
    public Sphere sphere;
    //[HideInInspector]
    public int countSphere;
    public int countSphereMax;
    float angle;
    public bool isStart;
    // Start is called before the first frame update
    void Start()
    {
        if (isStart)
        {
            StartCoroutine(CreateSphere());
        }
    }
    IEnumerator CreateSphere()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (countSphere < countSphereMax)
            {
                Sphere a = Instantiate(sphere);
                a.angle = angle;
                angle += 45;
                countSphere++;
            }
        }
    }
}
