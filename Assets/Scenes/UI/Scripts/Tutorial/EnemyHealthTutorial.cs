using FSMC.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthTutorial : MonoBehaviour
{
    //public EnemyState mob;
    TextAppear text;
    public bool isBoss;
    public bool isCutScene;
    FSMC_Executer mobScript;
    public LootManager loot;
    // Start is called before the first frame update
    void Start()
    {
        isCutScene = true;
        text = FindObjectOfType<TextAppear>();
        if (isBoss)
        {
            //mob.CurrentHealth = mob.MaxHealth;
            GetComponent<BossAttack>().damage = 0;
        }
        mobScript = GetComponent<FSMC_Executer>();
    }
    private void FixedUpdate()
    {
        if(isCutScene)
        {
            mobScript.MuteSpeed();
        }
        if (mobScript.health <= 0)
        {
            text.tutor.PhasePlus();
            text.tutor.BlockMoveAndShoot();

            if (isBoss)
            {
                //mob.Die();
                loot.DropLoot(true, transform);
            }
            Destroy(gameObject);
        }
    }
    
}
