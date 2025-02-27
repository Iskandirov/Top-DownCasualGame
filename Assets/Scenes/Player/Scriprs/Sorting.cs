using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RendererPriority
{
    public Renderer renderer;
    public int priority;
}
public class Sorting : MonoBehaviour
{
    [SerializeField] bool isStatic = false;
    [SerializeField] float offset = 0;
    int sortingOrderBase = 0;
    [SerializeField] RendererPriority[] renderers;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        index = 0;
        foreach (var rendererPriority in renderers)
        {
            rendererPriority.renderer.sortingOrder = (int)(sortingOrderBase + index - rendererPriority.renderer.transform.position.y + offset);
            index++;
        }

        if (isStatic)
            Destroy(this);
    }
    // Draw gizmos to visualize the offset
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 position = transform.position;
        position.y += offset;
        Gizmos.DrawLine(transform.position, position);
        Gizmos.DrawSphere(position, 0.1f);
    }
}
