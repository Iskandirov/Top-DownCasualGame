﻿using System.Collections.Generic;
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
    public TextMeshProUGUI count;
    public bool toSlot;
    public bool created;
    public bool second;

    GameObject targetEquipObjects;
    public bool toEquipSlot;
    private GameObject startParent;
    public GameObject equipPanel;
    LoadItemData itemData;

    public Button button;
    public bool isEquipedNow;
    public GameObject equipedChecker;
    public GameObject pointToCraft;
    public GameObject maxLevel;
    public GameObject LevelCount;

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
        startParent = transform.parent.gameObject;
        fliedSlots = FindObjectOfType<FieldSlots>();
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Finish");

        itemData = FindObjectOfType<LoadItemData>();
        gameManager = GameManager.Instance;

        //SetVisible(false);
        if (GetComponent<SetParametersToitem>().level != "4")
        {
            maxLevel.GetComponent<TagText>().tagText = "level";
            maxLevel.GetComponent<TextMeshProUGUI>().text = "Max level";
            LevelCount.SetActive(true);
            LevelCount.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<SetParametersToitem>().level;
        }
        list.Add(maxLevel);
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
        gameManager.UpdateText(list);
    }

    public void PointActivate()
    {
        int count = int.Parse(gameObject.GetComponent<SetParametersToitem>().Count.text);
        int level = int.Parse(gameObject.GetComponent<SetParametersToitem>().level);
        if (count >= 3 && level < 4)
        {
            pointToCraft.SetActive(true);
        }
        else if (count < 3 || level >= 4)
        {
            pointToCraft.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        if (isEquipedNow)
        {
            equipedChecker.SetActive(true);
        }
        else
        {
            equipedChecker.SetActive(false);
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
                    item.itemName.text = fliedSlots.objToCraft.ItemName;
                    item.level.text = fliedSlots.objToCraft.level;
                    item.stat = fliedSlots.objToCraft.ItemStat;
                    item.rare.text = fliedSlots.objToCraft.RareName;
                    item.description.text = fliedSlots.objToCraft.Description;
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
                        transform.localScale = new Vector3(.75f, .75f, .75f);
                        toEquipSlot = false;
                    }
                }
                else
                {
                    transform.SetParent(startParent.transform);
                    transform.SetAsFirstSibling();
                    transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);
                    transform.localScale = new Vector3(.45f, .45f, .45f);
                    toEquipSlot = true;
                    GameManager.Instance.ClosePanel(GameManager.Instance.menuPanel);
                }
            }
        }

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

        obj.level.text = obj.level.text == param.level ? obj.level.text : obj.level.text;
        obj.stat.text = obj.stat.text == param.ItemStat.text ? obj.stat.text : obj.stat.text;

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
            obj.Stat = checker.Stat == param.ItemStat.text ? checker.Stat : obj.Stat;
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
                //timer = 0f;
                transform.SetAsLastSibling(); // Äîäàéòå öåé ðÿäîê êîäó
            }
        }
        else
        {
            if (toSlot == true)
            {
                transform.localScale = targetScaleBig / 2;
                //timer = 0f;
                transform.SetAsFirstSibling(); // Äîäàéòå öåé ðÿäîê êîäó
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
                transform.SetAsFirstSibling(); // Äîäàéòå öåé ðÿäîê êîäó
            }
        }
        else
        {
            if (toSlot == true)
            {
                transform.localScale = targetScaleSmall / 2;
                transform.SetAsFirstSibling(); // Äîäàéòå öåé ðÿäîê êîäó
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