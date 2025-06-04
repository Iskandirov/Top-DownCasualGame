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
                    // отримуємо поточний розмір Image
                    Vector2 currentSize = objImage.rectTransform.sizeDelta;

                    // задаємо нову ширину, а висоту розраховуємо відповідно до пропорцій
                    float newWidth = 30f;
                    float newHeight = currentSize.y * (newWidth / currentSize.x);


                    float fullStat = float.Parse(data.Stat) + (float.Parse(data.Stat) * player.GivePerkStatValue(Stats.EquipmentBuff) / 100);
                    // встановлюємо новий розмір Image
                    objImage.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
                    countItems++;
                    switch (data.Name)
                    {
                        case "енергетичний заряд":
                            player.speed += fullStat;
                            break;
                        case "шестерня":
                            player.attackSpeed -= fullStat;
                            break;
                        case "етернійські-кристали":
                            player.damageToGive += fullStat;
                            break;
                        case "око дракона":
                            cam.m_Lens.OrthographicSize += fullStat;
                            break;
                        case "клик літучої миші":
                            player.isLifeSteal = true;
                            player.lifeStealPercent = fullStat;
                            break;
                        case "око дерев'яного снайпера":
                            player.launchForce += fullStat;
                            break; 
                        case "вижимка з грибів":
                            player.Wind += fullStat;
                            player.Water += fullStat;
                            player.Grass += fullStat;
                            player.Dirt += fullStat;
                            break;
                        case "корінь ківі":
                            player.isBulletSlow = true;
                            player.slowPercent += fullStat;
                            break;
                        case "слиз":
                            player.Fire += fullStat;
                            player.Cold -= fullStat;
                            break;
                        case "сфера бобса":
                            Debug.Log("sdf");
                            FindObjectOfType<SphereAround>().isStart = true;
                            FindObjectOfType<SphereAround>().sphere.damage += (float.Parse(data.Stat) * player.GivePerkStatValue(Stats.EquipmentBuff) / 100);
                            break;
                        case "щит роккі":
                            player.armor += fullStat;
                            break;
                    }
                }
            }
        }
    }
}
