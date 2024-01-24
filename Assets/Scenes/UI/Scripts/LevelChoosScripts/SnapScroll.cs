using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnapScroll : MonoBehaviour
{
    [Header("Object panel")]
    public FillLevelInfo panObj;
    public FillLevelInfo[] instObjects;
    public GameObject[] instObjectsLock;
    public GameObject instObjectsLockObj;
    public ScrollRect scrollRect;

    private RectTransform contentRect;

    [Header("Object parameters")]
    public Vector2[] instObjectsPosition;
    public Vector2[] instObjectsScale;
    public Vector2 contentVector;

    [Range(0f,10f)]
    public float snapSpeed;
    [Range(0f,10f)]
    public float scaleSpeed;
    [Range(0f,5f)]
    public float snapScale;
    [Header("Space between panels")]
    [Range(10, 500)]
    public int spacing;
    [Header("ID of curent active panel")]
    public int selectPanelObjID;
    public bool isScrolling;


    public Sprite[] descriptionImage;
    public int[] sceneValue;
    public TagText[] descriptionObjText;
    public Image[] descriptionObjImage;

    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        List<GameObject> list = new List<GameObject>();
        contentRect = GetComponent<RectTransform>();
        instObjects = new FillLevelInfo[descriptionImage.Length];
        instObjectsLock = new GameObject[descriptionImage.Length];
        instObjectsPosition = new Vector2[descriptionImage.Length];
        instObjectsScale = new Vector2[descriptionImage.Length];
        descriptionObjText = new TagText[descriptionImage.Length];
        descriptionObjImage = new Image[descriptionImage.Length];
        for (int i = 0; i < descriptionImage.Length; i++)
        {
            int objejectCount = instObjects[i].LoadObjectLevelCount(i + sceneValue[0]);
            int objejectCountMax = instObjects[i].LoadObjectLevelCountOfCountMax(i + sceneValue[0]);
            instObjects[i] = Instantiate(panObj, transform, false);
            instObjectsLock[i] = Instantiate(instObjectsLockObj, transform.position, Quaternion.identity, transform);
            if (instObjects[i].LoadObjectLevelCountIsFull(i + sceneValue[0]) == true 
                && objejectCount != objejectCountMax || objejectCount - 1 == objejectCountMax - 1 && objejectCount != objejectCountMax)
            {
                Destroy(instObjectsLock[i]);
            }
            descriptionObjText[i] = instObjects[i].GetComponentInChildren<TagText>();
            descriptionObjImage[i] = instObjects[i].GetComponentInChildren<Slider>().GetComponentInChildren<Image>();
            instObjects[i].GetComponent<MenuController>().sceneCount = sceneValue[i];
            if (i < descriptionImage.Length)
            {
                gameManager.FindMyComponentInChildren(descriptionObjText[i].gameObject, "description_lvl_" + i);
            }
            else
            {
                gameManager.FindMyComponentInChildren(descriptionObjText[i].gameObject, "description_lvl_last");
            }
            if (descriptionObjText[i].gameObject.GetComponent<TagText>())
            {
                list.Add(descriptionObjText[i].gameObject);
            }
            descriptionObjImage[i].sprite = descriptionImage[i];
            if (i == 0)
            {
                continue;
            }
            instObjects[i].objTransform.localPosition = new Vector2(instObjects[i - 1].objTransform.localPosition.x + panObj.GetComponent<RectTransform>().sizeDelta.x + spacing,
                instObjects[i].objTransform.localPosition.y);
            if (instObjectsLock[i] != null)
            {
                instObjectsLock[i].transform.localPosition = new Vector2(instObjectsLock[i - 1].transform.localPosition.x + panObj.GetComponent<RectTransform>().sizeDelta.x + spacing,
               instObjectsLock[i].transform.localPosition.y);
            }
           
            instObjectsPosition[i] = -instObjects[i].objTransform.localPosition;
        }
        gameManager.UpdateText(list);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (contentRect.anchoredPosition.x >= instObjectsPosition[0].x && !isScrolling ||
            contentRect.anchoredPosition.x <= instObjectsPosition[instObjectsPosition.Length - 1].x && !isScrolling)
        {
            isScrolling = false;
            scrollRect.inertia = false;
        }
        float nearestPos = float.MaxValue;
        for (int i = 0; i < descriptionImage.Length; i++)
        {
            float distance = Mathf.Abs(contentRect.anchoredPosition.x - instObjectsPosition[i].x);
            if (distance < nearestPos)
            {
                nearestPos = distance;
                selectPanelObjID = i;
            }
            float scale = Mathf.Clamp(10 / (distance / spacing) * snapScale, 0.5f, 10f);
            instObjectsScale[i].x = Mathf.SmoothStep(instObjects[i].objTransform.localRotation.x, scale, scaleSpeed * Time.fixedDeltaTime);
            instObjectsScale[i].y = Mathf.SmoothStep(instObjects[i].objTransform.localRotation.y, scale, scaleSpeed * Time.fixedDeltaTime);
            instObjects[i].transform.localScale = instObjectsScale[i];
            if (instObjectsLock[i] != null)
            {
                instObjectsLock[i].transform.localScale = instObjectsScale[i];
            }
        }
        float scrollVelocity = Mathf.Abs(scrollRect.velocity.x);
        if (scrollVelocity < 400 && !isScrolling) 
        {
            scrollRect.inertia= false;
        }
        if (isScrolling || scrollVelocity > 400)
        {
            return;
        }
        contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, instObjectsPosition[selectPanelObjID].x, snapSpeed * Time.fixedDeltaTime);
        contentRect.anchoredPosition = contentVector;
    }
    public void Scrolling(bool scroll)
    {
        isScrolling = scroll;
        if (scroll)
        {
            scrollRect.inertia = true;
        }
    }
}
