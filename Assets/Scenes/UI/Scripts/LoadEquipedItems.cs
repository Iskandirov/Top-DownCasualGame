using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class LoadEquipedItems : MonoBehaviour
{
    public List<GameObject> slots;
    public CinemachineVirtualCamera cam;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (System.IO.File.Exists(path))
        {
            string[] jsonLines = System.IO.File.ReadAllLines(path);

            foreach (string jsonLine in jsonLines)
            {
                SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(jsonLine);
                slots[i].transform.GetChild(0).GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(data.Name);
                slots[i].transform.GetChild(0).GetComponentInChildren<Image>().SetNativeSize();
                slots[i].transform.GetChild(0).GetComponentInChildren<Image>().rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                slots[i].transform.GetChild(0).GetComponentInChildren<Image>().rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                slots[i].transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(255f, 255f, 255f, 255f);
                // отримуємо поточний розмір Image
                Vector2 currentSize = slots[i].transform.GetChild(0).GetComponentInChildren<Image>().rectTransform.sizeDelta;

                // задаємо нову ширину, а висоту розраховуємо відповідно до пропорцій
                float newWidth = 30f;
                float newHeight = currentSize.y * (newWidth / currentSize.x);

                // встановлюємо новий розмір Image
                slots[i].transform.GetChild(0).GetComponentInChildren<Image>().rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
                i++;
                if (data.Name == "енергетичний заряд")
                {
                    GameObject speed = GameObject.Find("Player");
                    speed.GetComponent<Move>().speed += float.Parse(data.Stat);
                }
                if (data.Name == "шестерня")
                {
                    GameObject speed = GameObject.Find("Player");
                    speed.GetComponent<Shoot>().attackSpeed -= float.Parse(data.Stat);
                }
                if (data.Name == "етернійські-кристали")
                {
                    GameObject speed = GameObject.Find("Player");
                    speed.GetComponent<Shoot>().damageToGive += float.Parse(data.Stat);
                }
                if (data.Name == "око дракона")
                {
                    cam.m_Lens.OrthographicSize += float.Parse(data.Stat);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
