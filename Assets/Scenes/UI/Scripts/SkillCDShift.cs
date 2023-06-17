using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCDShift : MonoBehaviour
{
    public float skillCDShift;
    public float skillShiftCDMax;
    public Image spriteCD;
    public TextMeshProUGUI text;

    Move objMove;
    public void Start()
    {
        objMove = transform.root.GetComponent<Move>();
        skillShiftCDMax = objMove.dashTimeMax;
    }
    // Update is called once per frame
    void Update()
    {
        text.text = skillCDShift.ToString("0.");
        skillCDShift = objMove.dashTime;
        spriteCD.fillAmount = skillCDShift / skillShiftCDMax;
        if (skillCDShift <= 0)
        {
            text.gameObject.SetActive(false);
        }
        else
        {
            text.gameObject.SetActive(true);
        }
    }
}
