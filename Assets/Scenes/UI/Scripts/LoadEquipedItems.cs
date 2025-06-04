using Cinemachine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadEquipedItems : MonoBehaviour
{
    public List<GameObject> slots;
    public CinemachineVirtualCamera cam;
    public DataHashing hash;
    public int countItems = 0;
    // Start is called before the first frame update
    void Start()
    {
        PlayerManager player = FindObjectOfType<PlayerManager>();
        player.itemsFromBoss = this;
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
                    Image objImage = slots[countItems].transform.GetChild(0).GetComponentInChildren<Image>();
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


                    float fullStat = float.Parse(data.Stat) + (float.Parse(data.Stat) * player.GivePerkStatValue(Stats.EquipmentBuff) / 100);
                    // ������������ ����� ����� Image
                    objImage.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
                    countItems++;
                    switch (data.Name)
                    {
                        case "������������ �����":
                            player.speed += fullStat;
                            break;
                        case "��������":
                            player.attackSpeed -= fullStat;
                            break;
                        case "���������-��������":
                            player.damageToGive += fullStat;
                            break;
                        case "��� �������":
                            cam.m_Lens.OrthographicSize += fullStat;
                            break;
                        case "���� ����� ����":
                            player.isLifeSteal = true;
                            player.lifeStealPercent = fullStat;
                            break;
                        case "��� �����'����� ��������":
                            player.launchForce += fullStat;
                            break; 
                        case "������� � �����":
                            player.Wind += fullStat;
                            player.Water += fullStat;
                            player.Grass += fullStat;
                            player.Dirt += fullStat;
                            break;
                        case "����� ��":
                            player.isBulletSlow = true;
                            player.slowPercent += fullStat;
                            break;
                        case "����":
                            player.Fire += fullStat;
                            player.Cold -= fullStat;
                            break;
                        case "����� �����":
                            Debug.Log("sdf");
                            FindObjectOfType<SphereAround>().isStart = true;
                            FindObjectOfType<SphereAround>().sphere.damage += (float.Parse(data.Stat) * player.GivePerkStatValue(Stats.EquipmentBuff) / 100);
                            break;
                        case "��� ����":
                            player.armor += fullStat;
                            break;
                    }
                }
            }
        }
    }
}
