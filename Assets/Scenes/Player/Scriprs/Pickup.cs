using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            Transform child = FindChildWithScriptOfType(collision.transform, typeof(LightOn));
            if (child.GetComponent<LightOn>().IsPillBuffed == true)
            {
                Debug.Log(1);

                child.GetComponent<LightOn>().step += child.GetComponent<LightOn>().stepMax;
                child.GetComponent<LightOn>().IsPillUp = true;
                child.GetComponent<LightOn>().IsPillBuffed = true;
            }
            else 
            {
                Debug.Log(2);
                child.GetComponent<LightOn>().IsPillUp = true;
                child.GetComponent<LightOn>().IsPillBuffed = false;
            }
            
            Destroy(gameObject);
        }
    }
    // Рекурсивна функція для пошуку дочірнього елемента з певним скриптом
    Transform FindChildWithScriptOfType(Transform parent, System.Type scriptType)
    {
        var component = parent.GetComponent(scriptType);
        if (component != null)
            return parent;

        foreach (Transform child in parent)
        {
            var result = FindChildWithScriptOfType(child, scriptType);
            if (result != null)
                return result;
        }

        return null;
    }
}
