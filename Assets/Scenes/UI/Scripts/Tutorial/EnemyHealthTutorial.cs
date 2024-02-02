using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthTutorial : MonoBehaviour
{
    public Boss boss;
    TextAppear text;
    public float health;
    public bool isBoss;
    public GameObject healthBossObj;
    public GameObject uiParent;
    public GameObject healthObj;
    public Image healthBobsImg;
    // Start is called before the first frame update
    void Start()
    {
        text = FindObjectOfType<TextAppear>();
        if (isBoss)
        {
            uiParent = GameObject.Find("/UI");
            healthObj = Instantiate(healthBossObj, uiParent.transform);
            healthObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 2 - 100f, Screen.height / 2 - 130f);
            healthObj.GetComponentInChildren<Image>().fillAmount = 1;
        }
    }
    private void FixedUpdate()
    {
        if (health <= 0)
        {
            text.tutor.PhasePlus();
            text.tutor.BlockMoveAndShoot();

            if (isBoss)
            {
                boss.Death(new EnemyState());
                //GetComponent<DropItems>().isTutor = true;
                //GetComponent<DropItems>().OnDestroyBoss(healthBossObj);
            }
            Destroy(gameObject);
        }
    }

}
