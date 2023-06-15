using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCDShift : MonoBehaviour
{
    public float skillCD;
    public float skillCDMax;
    public Image spriteCD;
    public TextMeshProUGUI text;

    Move objMove;
    public void Start()
    {
        objMove = transform.root.GetComponent<Move>();
        skillCDMax = objMove.dashTimeMax;
    }
    // Update is called once per frame
    void Update()
    {
        text.text = skillCD.ToString("0.");
        skillCD = objMove.dashTime;
        spriteCD.fillAmount = skillCD / skillCDMax;
        if (skillCD <= 0)
        {
            text.gameObject.SetActive(false);
        }
        else
        {
            text.gameObject.SetActive(true);
        }
    }
}
