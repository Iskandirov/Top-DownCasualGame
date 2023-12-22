using System.Collections.Generic;
using UnityEngine;

public class SetLanguage : MonoBehaviour
{
    public List<GameObject> texts;
    public TagText FindMyComponentInChildren(GameObject parentObject,string tag)
    {
        TagText component = parentObject.GetComponent<TagText>();

        if (component != null)
        {
            component.tagText = tag;
            texts.Add(component.gameObject);
            return component;
        }

        foreach (Transform child in parentObject.transform)
        {
            component = FindMyComponentInChildren(child.gameObject, tag);

            if (component != null)
            {
                component.tagText = tag;
                texts.Add(component.gameObject);
                return component;
            }
        }

        return null;
    }
}
