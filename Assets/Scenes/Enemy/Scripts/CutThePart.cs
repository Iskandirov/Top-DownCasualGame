using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CutThePart : MonoBehaviour
{
    public List<CutThePart> parts;
    public GameObject vfxAttack;

    public void CutTheParts()
    {
        for (int i = parts.Count - 1; i >= 0; i--)
        {
            CutThePart part = parts[i];
            CutThePart obj = Instantiate(part, part.transform.position, Quaternion.identity);
            obj.AddComponent<MoveTowardsObject>();
            obj.GetComponent<MoveTowardsObject>().maxSpeed = 30;
            obj.GetComponent<MoveTowardsObject>().acceleration = 1;

            obj.AddComponent<Attack>();
            obj.GetComponent<Attack>().damage = 2;
            obj.GetComponent<Attack>().attackVFX = vfxAttack;
            obj.GetComponent<Attack>().stepAttack = 0.3f;
            obj.GetComponent<Attack>().stepAttackMax = 0.3f;
            obj.gameObject.tag = "Enemy";
            obj.GetComponent<Collider2D>().isTrigger = false;

            obj.GetComponent<MoveTowardsObject>().playerTransform = GameObject.FindWithTag("Player").gameObject.transform;
            obj.transform.localScale = new Vector3(2, 2, 2);
            obj.GetComponent<CutThePart>().parts.Clear();
            Destroy(part.gameObject);
            parts.RemoveAt(i);
        }
        //Debug.Log("Number of remaining elements in parts: " + parts.Count);
        //Debug.Log("Number of removed elements: " + (initialCount - parts.Count));
        if (gameObject.name == "Body")
        {
            Transform parentTransform = transform.root;
            Destroy(parentTransform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

