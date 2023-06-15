using Cinemachine;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadEquipedItems : MonoBehaviour
{
    public List<GameObject> slots;
    public CinemachineVirtualCamera cam;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (File.Exists(path))
        {
            string[] jsonLines = File.ReadAllLines(path);

            foreach (string jsonLine in jsonLines)
            {
                SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(jsonLine);
                Image objImage = slots[i].transform.GetChild(0).GetComponentInChildren<Image>();
                objImage.sprite = Resources.Load<Sprite>(data.Name);
                objImage.SetNativeSize();
                objImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                objImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                objImage.color = new Color(255f, 255f, 255f, 255f);
                // �������� �������� ����� Image
                Vector2 currentSize = objImage.rectTransform.sizeDelta;

                // ������ ���� ������, � ������ ����������� �������� �� ���������
                float newWidth = 30f;
                float newHeight = currentSize.y * (newWidth / currentSize.x);

                // ������������ ����� ����� Image
                objImage.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
                i++;
                switch (data.Name)
                {
                    case "������������ �����":
                        GameObject playerSpeed = GameObject.Find("Player");
                        playerSpeed.GetComponent<Move>().speed += float.Parse(data.Stat);
                        break;

                    case "��������":
                        GameObject playerShoot = GameObject.Find("Player");
                        playerShoot.GetComponent<Shoot>().attackSpeed -= float.Parse(data.Stat);
                        break;

                    case "���������-��������":
                        GameObject playerDamage = GameObject.Find("Player");
                        playerDamage.GetComponent<Shoot>().damageToGive += float.Parse(data.Stat);
                        break;

                    case "��� �������":
                        cam.m_Lens.OrthographicSize += float.Parse(data.Stat);
                        break;
                }
            }
        }
    }
}
