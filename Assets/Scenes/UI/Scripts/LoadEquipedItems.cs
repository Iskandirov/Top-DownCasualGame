using Cinemachine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadEquipedItems : MonoBehaviour
{
    public List<GameObject> slots;
    public CinemachineVirtualCamera cam;
    public DataHashing hash;
    // Start is called before the first frame update
    void Start()
    {
        PlayerManager player = PlayerManager.instance;
        int i = 0;
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (File.Exists(path))
        {
            string[] jsonLines = File.ReadAllLines(path);

            foreach (string jsonLine in jsonLines)
            {
                string decrypt = hash.Decrypt(jsonLine);
                if (decrypt != "")
                {
                    SavedObjectData data = JsonUtility.FromJson<SavedObjectData>(decrypt);
                    Image objImage = slots[i].transform.GetChild(0).GetComponentInChildren<Image>();
                    objImage.sprite = GameManager.ExtractSpriteListFromTexture("items").First(o => o.name == data.Name);
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
                            player.speed += float.Parse(data.Stat);
                            break;
                        case "��������":
                            player.attackSpeed -= float.Parse(data.Stat);
                            break;
                        case "���������-��������":
                            player.damageToGive += float.Parse(data.Stat);
                            break;
                        case "��� �������":
                            cam.m_Lens.OrthographicSize += float.Parse(data.Stat);
                            break;
                        case "���� ����� ����":
                            player.isLifeSteal = true;
                            player.lifeStealPercent = float.Parse(data.Stat);
                            break;
                        case "��� �����'����� ��������":
                            player.launchForce += float.Parse(data.Stat);
                            break; 
                        case "������� � �����":
                            player.Wind += float.Parse(data.Stat);
                            player.Water += float.Parse(data.Stat);
                            player.Grass += float.Parse(data.Stat);
                            player.Dirt += float.Parse(data.Stat);
                            break;
                        case "����� ��":
                            player.isBulletSlow = true;
                            player.slowPercent += float.Parse(data.Stat);
                            break;
                        case "����":
                            player.Fire += float.Parse(data.Stat);
                            player.Cold -= float.Parse(data.Stat);
                            break;
                        case "����� �����":
                            FindObjectOfType<SphereAround>().isStart = true;
                            break;
                        case "��� ����":
                            player.armor += float.Parse(data.Stat);
                            break;
                    }
                }
            }
        }
    }
}
