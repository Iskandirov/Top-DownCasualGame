using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public float health = 5;
    public List<GameObject> SpawnableObjects;

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Instantiate(SpawnableObjects[Random.Range(0, SpawnableObjects.Count)], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
