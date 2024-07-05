using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthTutorial : MonoBehaviour
{
    public Boss mob;
    TextAppear text;
    public bool isBoss;
    public GameObject healthBossObj;
    public Image healthBobsImg;
    EnemyState state;
    // Start is called before the first frame update
    void Start()
    {
        text = FindObjectOfType<TextAppear>();
        if (isBoss)
        {
            mob.health = mob.SetBase();
            GetComponent<BossAttack>().damage = 0;
        }
        state = GetComponent<EnemyState>();
    }
    private void FixedUpdate()
    {
        if (state.health <= 0)
        {
            text.tutor.PhasePlus();
            text.tutor.BlockMoveAndShoot();

            if (isBoss)
            {
                mob.Death(state);
                //GetComponent<DropItems>().isTutor = true;
                //GetComponent<DropItems>().OnDestroyBoss(healthBossObj);
            }
            Destroy(gameObject);
        }
    }
}
