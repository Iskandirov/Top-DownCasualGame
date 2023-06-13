using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler//, IPointerUpHandler
{
    private Vector3 startPosition;
    private Vector3 startScale;
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

    //float duration = 2f;
    //float timer = 0f;
    [Range(0, 50)]
    public float XY = 2f;
    Vector3 targetScaleBig;
    Vector3 targetScaleSmall;

    public GameObject Sibling;
    public GameObject lang;
    List<GameObject> list = new List<GameObject>();

    void Start()
    {
        startParent = transform.parent.gameObject;
        fliedSlots = FindObjectOfType<FieldSlots>();
        // Знайти об'єкт за тегом
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Finish");
        targetEquipObjects = GameObject.FindGameObjectWithTag("Respawn");
        equipPanel = GameObject.FindGameObjectWithTag("Wall");
        itemData = GameObject.FindGameObjectWithTag("Lightning");
        lang = GameObject.Find("Main Camera");
        SetVisible(false);
        if (gameObject.GetComponent<SetParametersToitem>().level != "4")
        {
            maxLevel.GetComponent<TagText>().tagText = "level";
            maxLevel.GetComponent<TextMeshProUGUI>().text = "Рівень ";
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
        else
        {
            startScale = Sibling.GetComponent<MoveItem>().transform.localScale;
            targetScaleBig = Sibling.GetComponent<MoveItem>().startScale * 1.1f;
            targetScaleSmall = Sibling.GetComponent<MoveItem>().startScale / 1.1f;
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
    public void MoveIt()
    {
        if (toEquipSlot == transform)
        {
            //Set item card to craft slots
            if (toSlot == true && toEquipSlot)
            {
                targetObjects.Sort((x, y) => x.name.CompareTo(y.name));
                foreach (var obj in targetObjects)
                {
                    Collider2D[] colliders = Physics2D.OverlapPointAll(obj.transform.position);
                    bool hasGameController = false;
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.gameObject.CompareTag("GameController"))
                        {
                            hasGameController = true;
                            break;
                        }
                    }
                    if (!hasGameController)
                    {
                        int i = int.Parse(count.text);
                        if (i > 1) // if there is more than one card
                        {

                            GameObject card = Instantiate(gameObject, transform.parent);

                            card.GetComponent<MoveItem>().created = true;
                            card.GetComponent<MoveItem>().count.text = "1";
                            card.GetComponent<MoveItem>().toSlot = false;
                            card.GetComponent<MoveItem>().second = true;
                            card.GetComponent<MoveItem>().Sibling = gameObject;
                            fliedSlots.objToCraft.Add(card);

                            card.transform.SetParent(obj.transform);
                            card.transform.position = obj.transform.position;
                            //card.transform.localScale = startScale;
                            if (int.TryParse(gameObject.transform.parent.transform.GetComponentInChildren<MoveItem>().count.text, out int parentCount)
                                && int.TryParse(card.GetComponent<MoveItem>().count.text, out int itemCount))
                            {
                                int y = parentCount - itemCount;
                                transform.parent.transform.GetComponentInChildren<MoveItem>().count.text = y.ToString();
                            }
                            else
                            {
                                Debug.LogError("Count text is not a valid number!");
                            }
                            fliedSlots.CheckCraft();
                            break;
                        }
                        else // if it`s only one card
                        {
                            transform.SetParent(obj.transform);
                            //transform.localScale = targetScaleSmall;
                            transform.position = obj.transform.position;
                            toSlot = false;
                            fliedSlots.objToCraft.Add(gameObject);
                            fliedSlots.CheckCraft();
                            break;
                        }
                    }
                }
            }
            else if (toEquipSlot)
            {

                if (created)
                {
                    if (Sibling.GetComponent<MoveItem>().toSlot == true)
                    {
                        if (int.TryParse(Sibling.GetComponent<MoveItem>().count.text, out int parentCount)
                            && int.TryParse(count.text, out int itemCount))
                        {
                            int i = parentCount + itemCount;
                            Sibling.GetComponent<MoveItem>().count.text = i.ToString();
                            fliedSlots.objToCraft.Remove(gameObject);
                            fliedSlots.allItemsHaveSameName = true;
                            Destroy(gameObject);
                        }
                        else
                        {
                            Debug.LogError("Count text is not a valid number!");
                        }
                    }
                    else if (Sibling.GetComponent<MoveItem>().toSlot == false)
                    {
                        transform.SetParent(Sibling.GetComponent<MoveItem>().startParent.transform);
                        transform.localScale = Sibling.GetComponent<MoveItem>().startScale;
                        transform.position = new Vector3(Sibling.GetComponent<MoveItem>().startParent.transform.position.x, Sibling.GetComponent<MoveItem>().startParent.transform.position.y + 1f, Sibling.GetComponent<MoveItem>().startParent.transform.position.z);
                        toSlot = true;
                        second = true;
                        fliedSlots.objToCraft.Remove(gameObject);
                        fliedSlots.allItemsHaveSameName = true;
                    }
                }
                else
                {
                    MoveItem[] children = startParent.GetComponentsInChildren<MoveItem>();
                    if (children.Length > 0)
                    {
                        foreach (MoveItem child in children)
                        {
                            if (child.second == true && child.toSlot == true)
                            {
                                if (int.TryParse(child.count.text, out int parentCount)
                                    && int.TryParse(count.text, out int itemCount))
                                {

                                    int i = parentCount + itemCount;
                                    transform.SetParent(startParent.transform);
                                    count.text = i.ToString();
                                    transform.localScale = targetScaleBig;
                                    transform.position = new Vector3(startParent.transform.position.x, startParent.transform.position.y + 1f, startParent.transform.position.z);
                                    toSlot = true;
                                    fliedSlots.objToCraft.Remove(gameObject);
                                    fliedSlots.allItemsHaveSameName = true;
                                    Destroy(child.gameObject);
                                }
                                else
                                {
                                    Debug.LogError("Count text is not a valid number!");
                                }
                                break;
                            }

                        }
                    }
                    else if (children.Length == 0)
                    {
                        transform.SetParent(startParent.transform);
                        transform.localScale = targetScaleBig;
                        transform.position = new Vector3(startParent.transform.position.x, startParent.transform.position.y + 1f, startParent.transform.position.z);
                        toSlot = true;
                        fliedSlots.objToCraft.Remove(gameObject);
                        fliedSlots.allItemsHaveSameName = true;
                    }
                }
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !Input.GetMouseButtonUp(0) && Input.GetMouseButtonUp(1))
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
            if (equipPanel.transform.GetChild(i).GetComponent<Button>())
            {
                equipPanel.transform.GetChild(i).GetComponent<Image>().enabled = isActivate;
                equipPanel.transform.GetChild(i).GetComponent<Button>().enabled = isActivate;
                button = equipPanel.transform.GetChild(i).GetComponent<Button>();
                equipPanel.transform.GetChild(i).GetComponent<Button>().transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().enabled = isActivate;
            }
            else if (equipPanel.transform.GetChild(i).GetComponent<Image>())
            {
                equipPanel.transform.GetChild(i).GetComponent<Image>().enabled = isActivate;
            }
            else if (equipPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>())
            {
                equipPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>().enabled = isActivate;
                if (isActivate == true)
                {
                    foreach (var obj in itemData.GetComponent<LoadItemData>().objectsList)
                    {
                        if (equipPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>().name == "Stat")
                        {
                            SetItem(obj);
                            string path = Path.Combine(Application.persistentDataPath, "EquipedItems.txt");
                            if (File.Exists(path))
                            {
                                string[] jsonLines = File.ReadAllLines(path);
                                foreach (string jsonLine in jsonLines)
                                {
                                    SavedEquipData data = JsonUtility.FromJson<SavedEquipData>(jsonLine);
                                    if (data.Tag == stats[1].GetComponent<TagText>().tagText)
                                    {
                                        button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Зняти";
                                        isEquipedNow = true;
                                        break;
                                    }
                                    else
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
                transform.SetAsLastSibling(); // Додайте цей рядок коду
            }
        }
        else
        {
            if (toSlot == true)
            {
                transform.localScale = targetScaleBig / 2;
                //timer = 0f;
                transform.SetAsFirstSibling(); // Додайте цей рядок коду
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
                transform.SetAsFirstSibling(); // Додайте цей рядок коду
            }
        }
        else
        {
            if (toSlot == true)
            {
                transform.localScale = targetScaleSmall / 2;
                transform.SetAsFirstSibling(); // Додайте цей рядок коду
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