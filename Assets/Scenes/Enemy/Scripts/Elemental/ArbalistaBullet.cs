using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArbalistaBullet : MonoBehaviour
{
    public float speed = 10f;
    public Vector2 direction;
    public Vector2 lastContactPoint;
    public Vector2 lastContactNormal;
    public int maxBounces = 5;
    private Rigidbody rb;

    public LayerMask collisionMask;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.velocity = direction.normalized * speed;
        RotateTowardsDirection(direction);
        // Заблокуй рух по осі Z
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                | RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationY
                | RigidbodyConstraints.FreezeRotationZ;
    }
    void Update()
    {
        //rb.velocity = direction.normalized * speed;
        transform.Translate(Vector3.up * Time.deltaTime * speed);

        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Time.deltaTime * speed + .1f, collisionMask))
        {
            Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
            transform.up = reflectDir.normalized;
        }
       
    }
    void RotateTowardsDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.black;
        Gizmos.DrawRay(lastContactPoint, lastContactNormal);
    }
    
}
