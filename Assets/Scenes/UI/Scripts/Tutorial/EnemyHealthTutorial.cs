using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthTutorial : MonoBehaviour
{
    public Boss mob;
    TextAppear text;
    public bool isBoss;
    public GameObject healthBossObj;
    public Image healthBobsImg;
    // Start is called before the first frame update
    void Start()
    {
        text = FindObjectOfType<TextAppear>();
        if (isBoss)
        {
            mob.healthBossObj = mob.SetBase();
        }
    }
    private void FixedUpdate()
    {
        if (mob.healthMax <= 0)
        {
            text.tutor.PhasePlus();
            text.tutor.BlockMoveAndShoot();

            if (isBoss)
            {
                mob.Death(GetComponent<EnemyState>());
                //GetComponent<DropItems>().isTutor = true;
                //GetComponent<DropItems>().OnDestroyBoss(healthBossObj);
            }
            Destroy(gameObject);
        }
    }
}
