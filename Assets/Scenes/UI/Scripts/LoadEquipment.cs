using System.Collections.Generic;
using UnityEngine;

public class LoadEquipment : MonoBehaviour
{
    public RectTransform parent;
    public GameObject objToSet;
    public List<GameObject> list;

    public void SetEquip(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject a = Instantiate(objToSet, transform.position, new Quaternion(0, 0, 0, 0), parent);
            list.Add(a);
        }
    }
    public void DeleteList(List<GameObject> list)
    {
        // Видалення всіх об'єктів зі списку
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }

        // Очищення списку
        list.Clear();
    }
}
