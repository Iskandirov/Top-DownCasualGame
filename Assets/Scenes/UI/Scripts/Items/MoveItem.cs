using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveItem : MonoBehaviour ,IPointerClickHandler
{
    public Vector3 startPosition;

    public Vector3 startScale;
    public List<GameObject> targetObjects;
    FieldSlots fliedSlots;
    public bool toSlot;
    public bool created;
    public bool second;

    GameObject targetEquipObjects;
    public bool toEquipSlot;
    public Shelf startParent;
    public GameObject equipPanel;
    LoadItemData itemData;

    public Button button;
    public bool isEquipedNow;

    [Range(0, 50)]
    public float XY = 2f;
    Vector3 targetScaleBig;
    Vector3 targetScaleSmall;

    public GameManager gameManager;
    List<GameObject> list = new List<GameObject>();
    public DataHashing hashing;
    void Start()
    {
        hashing = FindObjectOfType<DataHashing>();
       
        fliedSlots = FindObjectOfType<FieldSlots>();
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Finish");

        itemData = FindObjectOfType<LoadItemData>();
        gameManager = GameManager.Instance;
        
        foreach (GameObject obj in objectsWithTag)
        {
            targetObjects.Add(obj);
        }
        startPosition = transform.parent.position;

        startScale = transform.localScale;
        targetScaleBig = startScale * 1.1f;
        targetScaleSmall = startScale / 1.1f;
        toSlot = true;
        toEquipSlot = true;
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        lock (new object())
        {
            if (File.Exists(path))
            {
                string[] jsonLines = File.ReadAllLines(path);

                foreach (string jsonLine in jsonLines)
                {
                    string decrypt = hashing.Decrypt(jsonLine);
                    SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(decrypt);
                    if (data.Tag == GetComponent<SetParametersToitem>().Tag)
                    {
                        isEquipedNow = true;
                    }
                }
            }
        }
        PointActivate();
       
        startParent.count.text = GetComponent<SetParametersToitem>().Count;
        startParent.level.text = GetComponent<SetParametersToitem>().level != "4" ? gameObject.GetComponent<SetParametersToitem>().level : "Max";
        gameManager.UpdateText(list);
    }

    public void PointActivate()
    {
        startParent = transform.parent.GetComponent<Shelf>();
        int count = int.Parse(GetComponent<SetParametersToitem>().Count);
        int level = int.Parse(GetComponent<SetParametersToitem>().level);
        if (count >= 3 && level < 4)
        {
            startParent.pointToUpgrade.SetActive(true);
        }
        else if (count < 3 || level >= 4)
        {
            startParent.pointToUpgrade.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        if (isEquipedNow)
        {
            startParent.checker.SetActive(true);
        }
        else
        {
            startParent.checker.SetActive(false);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)
        {

            if (toSlot == true)
            {
                if (toEquipSlot)
                {
                    GameManager.Instance.OpenPanel(GameManager.Instance.menuPanel,false);
                    bool hasGameController = false;
                    targetEquipObjects = GameObject.FindGameObjectWithTag("Respawn");
                    equipPanel = GameObject.FindGameObjectWithTag("Wall");

                    //Перший в порядку
                    fliedSlots.objToCraft = GetComponent<SetParametersToitem>();
                    fliedSlots.CheckCraft();

                    ItemData item = equipPanel.GetComponent<ItemData>();
                    item.itemName.GetComponent<TagText>().tagText = fliedSlots.objToCraft.Tag;
                    item.level.text = fliedSlots.objToCraft.level;
                    item.stat.text = fliedSlots.objToCraft.ItemStat;
                    item.rare.GetComponent<TagText>().tagText = fliedSlots.objToCraft.RareTag;
                    item.description.GetComponent<TagText>().tagText = "descriprion_info_" + fliedSlots.objToCraft.Tag;//fliedSlots.objToCraft.Description;
                    item.itemTag = fliedSlots.objToCraft.Tag;
                    item.raretag = fliedSlots.objToCraft.RareTag;
                    //Другий в порядку
                    SetCardInfo();

                    if (targetEquipObjects.CompareTag("GameController"))
                    {
                        hasGameController = true;
                    }
                    if (!hasGameController)
                    {
                        transform.SetParent(targetEquipObjects.transform);
                        transform.position = targetEquipObjects.transform.position;
                        transform.localScale = transform.localScale * 1.5f;
                        toEquipSlot = false;
                    }
                }
                else
                {
                    transform.SetParent(startParent.transform);
                    transform.SetAsFirstSibling();
                    transform.localPosition = Vector3.zero;
                    transform.localScale = startScale;
                    toEquipSlot = true;
                    GameManager.Instance.ClosePanel(GameManager.Instance.menuPanel);
                }
            }
        }
        gameManager.UpdateText(GameManager.Instance.texts);
    }
    public void SetCardInfo()
    {
        ItemData itemData = equipPanel.GetComponent<ItemData>();
        SetItem(itemData);


        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
        using (var streamReader = new StreamReader(fileStream))
        {
            lock (fileStream)
            {
                if (File.Exists(path))
                {
                    string[] jsonLines = File.ReadAllLines(path);
                    bool foundMatch = false;

                    foreach (string jsonLine in jsonLines)
                    {
                        string decrypt = hashing.Decrypt(jsonLine);
                        SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(decrypt);
                        if (data.Tag == itemData.itemName.GetComponent<TagText>().tagText)
                        {
                            itemData.state.text = "Зняти";
                            isEquipedNow = true;
                            foundMatch = true;
                            break;
                        }
                    }

                    if (!foundMatch)
                    {
                        itemData.state.text = "Обладнати";
                        isEquipedNow = false;
                    }
                }
            }
        }
    }
    public void SetItem(ItemData obj)
    {
        SetParametersToitem param = GetComponent<SetParametersToitem>();

        //obj.level.text = obj.level.text == param.level ? obj.level.text : obj.level.text;
        //obj.stat.text = obj.stat.text == param.ItemStat ? obj.stat.text : obj.stat.text;

        if (obj.itemName.text == param.ItemName)
        {
            obj.itemName.text = param.ItemName;
            obj.itemName.GetComponent<TagText>().tagText = param.Tag;
            list.Add(obj.itemName.gameObject);
        }
        if (obj.rare.text == param.RareName)
        {
            obj.rare.text = param.RareName;
            obj.rare.GetComponent<TagText>().tagText = param.RareTag;
            list.Add(obj.rare.gameObject);
        }
        gameManager.UpdateText(list);
    }
    public SavedEquipData SetItem()
    {
        SavedEquipData obj = new SavedEquipData();
        foreach (var checker in itemData.GetComponent<LoadItemData>().objectsList)
        {
            SetParametersToitem param = GetComponent<SetParametersToitem>();

            obj.Level = checker.Level.ToString() == param.level ? checker.Level : obj.Level;
            obj.Name = checker.Name == param.ItemName ? checker.Name : obj.Name;
            obj.Stat = checker.Stat == param.ItemStat ? checker.Stat : obj.Stat;
            obj.Tag = checker.Tag == param.Tag ? checker.Tag : obj.Tag;
        }
        return obj;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && toEquipSlot)
        {
            if (toSlot == true)
            {
                transform.localScale = targetScaleBig;
                transform.SetAsLastSibling(); 
            }
        }
        else
        {
            if (toSlot == true)
            {
                transform.localScale = targetScaleBig / 2;
                transform.SetAsFirstSibling(); 
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && toEquipSlot)
        {
            if (toSlot == true)
            {
                transform.localScale = targetScaleSmall;
                transform.SetAsFirstSibling();
            }
        }
        else
        {
            if (toSlot == true)
            {
                transform.localScale = targetScaleSmall / 2;
                transform.SetAsFirstSibling(); 
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !Input.GetMouseButton(1))
        {
            transform.localScale = transform.localScale / 1.1f;
        }
    }
}