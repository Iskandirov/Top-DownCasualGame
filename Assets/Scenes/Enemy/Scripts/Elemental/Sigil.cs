using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class Sigil : MonoBehaviour
{
    public Image sigilImage;
    public List<Color32> colorVariants;
    public List<Color32> colorVariantsStone;
    public List<Sprite> spriteVariants;
    public SpriteRenderer elementImg;
    public SpriteRenderer SigilImg;
    public Image SigilWiresImg;
    public status targetStatus;
    public GameObject abilParent;
    public Animator anim;
    bool inZone;
    HashSet<status> uniqueStatuses = new HashSet<status>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var abil in abilParent.GetComponentsInChildren<CDSkills>())
        {
            foreach (var s in abil.status) // якщо в абілці може бути кілька статусів
            {
                uniqueStatuses.Add(s);
            }
        }
        List<status> statusList = uniqueStatuses.ToList(); // конвертуємо в список для індексації
        System.Random random = new System.Random();
        targetStatus = statusList[random.Next(statusList.Count)];
        sigilImage.color = colorVariants[(int)targetStatus];
        elementImg.color = colorVariantsStone[(int)targetStatus];
        SigilImg.color = colorVariantsStone[(int)targetStatus];
        SigilWiresImg.color = colorVariantsStone[(int)targetStatus];
    }

    // Update is called once per frame
    void Update()
    {
        if(sigilImage.fillAmount == 1)
        {
            //sigilImage.fillAmount = 0;
            anim.SetTrigger("FullyCharged");
            Debug.Log("Sigil fully charged");
        }
        if (!inZone && sigilImage.fillAmount == 1)
        {
            sigilImage.fillAmount -= .002f;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            SkillBaseMono skill = collision.gameObject.GetComponent<SkillBaseMono>();
            Debug.Log($"Skill: {skill.name}, Target Status: {skill.Elements[0]}");
            int matchCount = skill.Elements.Count(e => e == targetStatus);
            if (matchCount > 0)
            {
                inZone = true;
                sigilImage.fillAmount += .01f * matchCount;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            inZone = false;
           
        }
    }
    public void HideSigil()
    {
        if (GetComponentInChildren<Image>() != null)
        {
            SigilImg.gameObject.SetActive(false);
            GetComponentInChildren<Image>().gameObject.SetActive(false);
        }
    }
}
