using System.Collections;
using UnityEngine;

public class DeactivateDebuff : MonoBehaviour
{
    public float lifeTime;
    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy();
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
