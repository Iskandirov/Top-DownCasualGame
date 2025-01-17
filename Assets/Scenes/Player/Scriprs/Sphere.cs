using FSMC.Runtime;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public float rotationSpeed = 500f;
    public float circleRadius;
    public float speed;
    public float angle;
    public float damage;
    Transform objTransform;
    PlayerManager player;
    private void Start()
    {
        objTransform = transform;
        player = PlayerManager.instance;
    }
    private void FixedUpdate()
    {
        rotationSpeed += speed * 5;
        // Оновлення кута обертання об'єкта
        objTransform.rotation = Quaternion.Euler(objTransform.rotation.x, objTransform.rotation.y, rotationSpeed);

        objTransform.position = player.objTransform.position + new Vector3(Mathf.Cos(Time.time * speed + angle) * circleRadius, Mathf.Sin(Time.time * speed + angle) * circleRadius, 0f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            collision.GetComponent<FSMC_Executer>().TakeDamage(damage);
            FindObjectOfType<SphereAround>().countSphere--;
            Destroy(gameObject);

        }
        else if (collision.CompareTag("Barrel"))
        {
            collision.GetComponent<ObjectHealth>().TakeDamage();
            FindObjectOfType<SphereAround>().countSphere--;
            Destroy(gameObject);
        }
    }
}
