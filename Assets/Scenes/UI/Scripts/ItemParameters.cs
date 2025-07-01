using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemParameters : MonoBehaviour
{
    public string itemName;
    public Sprite itemRare;
    public int idRare;
    public string itemRareName;
    public string Stat;
    public int Level;
    public int Price;
    public string Tag;
    public string RareTag;
    public string Description;
    [SerializeField]
    DataHashing hash;
    Timer dropLooted;
    public ItemParameters itemPanel;
    public List<GameObject> panelObjs;
    public bool isTutor;
    [Header("Panels info")]
    public Image itemImage;
    public TextMeshProUGUI itemNameTxt;
    public TextMeshProUGUI itemDescription;
    private bool isCollected = false;
    // Start is called before the first frame update
    void Start()
    {
        hash = DataHashing.inst;
        dropLooted = FindObjectOfType<Timer>();
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;
        Tutor tut = FindObjectOfType<Tutor>();
        if (collision.CompareTag("Player") && !collision.isTrigger && tut == null || tut?.phase == 6)
        {
            isCollected = true;

            SaveItem();
            ItemParameters item = Instantiate(itemPanel, itemPanel.transform.position, Quaternion.identity, itemPanel.transform.parent);
            //GameManager.Instance.OpenPanel(item.gameObject);
            item.gameObject.SetActive(true);
            LootManager.SetPanelsInfoToLoot(this, item);
            itemImage.sprite = GameManager.ExtractSpriteListFromTexture("items").First(instance => instance.name == itemName);
            itemImage.SetNativeSize();
            var rectTransform = itemImage.GetComponent<RectTransform>();
            //rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * 5, rectTransform.sizeDelta.y * 5);
            itemNameTxt.text = itemName;
            itemDescription.text = Description;
            Destroy(gameObject);
            // Уведомляем LootManager о сборе предмета
            
            return;
        }
    }
    public void EndGame()
    {
        GameManager.Instance.ClosePanel(gameObject);
        LootManager.inst.ItemCollected();
        if (LootManager.inst.activeItemsCount <= 0)
        {
            if (!isTutor)
            {
                dropLooted.isDropLooted = true;
            }
            else
            {
                Tutor tut = FindObjectOfType<Tutor>();
                tut.PhasePlus();
                tut.BlockMoveAndShoot();
            }
            if (AudioManager.instance != null)
            {
                AudioManager.instance.MusicStop();
                AudioManager.instance.PlaySFX("Win");
            }
        }
    }
    void SaveItem()
    {
        SavedObjectData data = new SavedObjectData();
        data.Name = itemName;
        data.IDRare = idRare;
        data.RareName = itemRareName;
        data.Stat = Stat;
        data.Level = Level;
        data.Tag = Tag;
        data.RareTag = RareTag;
        data.Description = Description;
        // Збереження даних у файл
        string fileName = Path.Combine(Application.persistentDataPath, "savedData.txt");
        using (StreamWriter writer = new StreamWriter(fileName, true))
        {
            string jsonData = JsonUtility.ToJson(data);
            string decryptedJson = hash.Encrypt(jsonData);
            writer.WriteLine(decryptedJson);
            writer.Close();
        }
    }
}
