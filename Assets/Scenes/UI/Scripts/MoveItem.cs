using System;
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
    public TextMeshProUGUI count;
    public bool toSlot;
    public bool created;
    public bool second;

    GameObject targetEquipObjects;
    public bool toEquipSlot;
    private GameObject startParent;
    GameObject equipPanel;
    GameObject itemData;
    public List<TextMeshProUGUI> stats;

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

    public GameObject lang;
    List<GameObject> list = new List<GameObject>();
    void Start()
    {
        startParent = transform.parent.gameObject;
        fliedSlots = FindObjectOfType<FieldSlots>();
        // Çíàéòè îá'ºêò çà òåãîì
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Finish");
        targetEquipObjects = GameObject.FindGameObjectWithTag("Respawn");
        equipPanel = GameObject.FindGameObjectWithTag("Wall");
        itemData = GameObject.FindGameObjectWithTag("Lightning");
        lang = GameObject.Find("Main Camera");
        SetVisible(false);
        if (gameObject.GetComponent<SetParametersToitem>().level != "4")
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

        if (!created)
        {
            startScale = transform.localScale;
            targetScaleBig = startScale * 1.1f;
            targetScaleSmall = startScale / 1.1f;
            toSlot = true;
            toEquipSlot = true;
        }
        string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
        if (File.Exists(path))
        {
            string[] jsonLines = File.ReadAllLines(path);
            foreach (string jsonLine in jsonLines)
            {
                SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(jsonLine);
                if (data.Tag == gameObject.GetComponent<SetParametersToitem>().Tag)
                {
                    isEquipedNow = true;
                }
            }
        }
        PointActivate();
        lang.GetComponent<SetLanguage>().settings.UpdateText(list);
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
            if (!created && toSlot == true)
            {

                if (toEquipSlot == true)
                {
                    bool hasGameController = false;
                    if (targetEquipObjects.CompareTag("GameController"))
                    {
                        hasGameController = true;
                    }
                    if (!hasGameController)
                    {
                        SetVisible(true);
                        transform.SetParent(targetEquipObjects.transform);
                        //transform.localScale = targetScaleSmall;
                        transform.position = targetEquipObjects.transform.position;
                        toEquipSlot = false;
                    }
                }
                else
                {
                    transform.SetParent(startParent.transform);
                    transform.SetAsFirstSibling();
                    //transform.localScale = startScale;
                    transform.position = new Vector3(startPosition.x, startPosition.y + 1f, startPosition.z);
                    toEquipSlot = true;
                    SetVisible(false);
                }
            }
            fliedSlots.objToCraft = GetComponent<SetParametersToitem>();
            fliedSlots.CheckCraft();
        }

    }

    public void SetVisible(bool isActivate)
    {
        equipPanel.GetComponent<Image>().enabled = isActivate;
        if (stats.Count == 0)
        {
            for (int i = 0; i < equipPanel.transform.childCount; i++)
            {
                if (equipPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>())
                {
                    equipPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>().enabled = isActivate;
                    if (isActivate == true)
                    {
                        if (equipPanel.transform.GetChild(i).CompareTag("Stat"))
                        {
                            stats.Add(equipPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
                        }
                    }
                }
            }
        }
        for (int i = 0; i < equipPanel.transform.childCount; i++)
        {
            Transform child = equipPanel.transform.GetChild(i);

            if (child.GetComponent<Button>())
            {
                child.GetComponent<Image>().enabled = isActivate;
                child.GetComponent<Button>().enabled = isActivate;

                if (child.name == "EquipBtn")
                {
                    button = child.GetComponent<Button>();
                }

                child.GetComponentInChildren<TextMeshProUGUI>().enabled = isActivate;
            }
            else if (child.GetComponent<Image>())
            {
                child.GetComponent<Image>().enabled = isActivate;
            }
            else if (child.GetComponent<TextMeshProUGUI>())
            {
                child.GetComponent<TextMeshProUGUI>().enabled = isActivate;

                if (isActivate)
                {
                    foreach (var obj in itemData.GetComponent<LoadItemData>().objectsList)
                    {
                        if (child.name == "Stat")
                        {
                            SetItem(obj);
                            string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");

                            if (File.Exists(path))
                            {
                                string[] jsonLines = File.ReadAllLines(path);
                                bool foundMatch = false;

                                foreach (string jsonLine in jsonLines)
                                {
                                    SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(jsonLine);

                                    if (data.Tag == stats[1].GetComponent<TagText>().tagText)
                                    {
                                        button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Зняти";
                                        isEquipedNow = true;
                                        foundMatch = true;
                                        break;
                                    }
                                }

                                if (!foundMatch)
                                {
                                    button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Обладнати";
                                    isEquipedNow = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public void SetItem(SavedObjectData obj)
    {
        if (obj.Level.ToString() == gameObject.GetComponent<SetParametersToitem>().level)
        {
            stats[0].text = obj.Level.ToString();
        }
        if (obj.Name == gameObject.GetComponent<SetParametersToitem>().ItemName)
        {
            stats[1].text = obj.Name;
            stats[1].GetComponent<TagText>().tagText = gameObject.GetComponent<SetParametersToitem>().Tag;
            list.Add(stats[1].gameObject);
        }
        if (obj.Stat == gameObject.GetComponent<SetParametersToitem>().ItemStat.text)
        {
            stats[2].text = obj.Stat;
        }
        if (obj.RareName == gameObject.GetComponent<SetParametersToitem>().RareName)
        {
            stats[3].text = obj.RareName;
            stats[3].GetComponent<TagText>().tagText = gameObject.GetComponent<SetParametersToitem>().RareTag;
            list.Add(stats[3].gameObject);
        }
        lang.GetComponent<SetLanguage>().settings.UpdateText(list);
    }
    public void SetItem(SavedEquipData obj)
    {
        foreach (var checker in itemData.GetComponent<LoadItemData>().objectsList)
        {
            if (checker.Level.ToString() == gameObject.GetComponent<SetParametersToitem>().level)
            {
                obj.Level = checker.Level;
            }
            if (checker.Name == gameObject.GetComponent<SetParametersToitem>().ItemName)
            {
                obj.Name = checker.Name;
            }
            if (checker.Stat == gameObject.GetComponent<SetParametersToitem>().ItemStat.text)
            {
                obj.Stat = checker.Stat;
            }
            if (checker.Tag == gameObject.GetComponent<SetParametersToitem>().Tag)
            {
                obj.Tag = checker.Tag;
            }
        }
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
        //if (eventData.button == PointerEventData.InputButton.Right && !Input.GetMouseButton(0))
        //{
        //    transform.localScale = targetScaleSmall;
        //}
    }
}