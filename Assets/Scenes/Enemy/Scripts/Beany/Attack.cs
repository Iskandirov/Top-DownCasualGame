using FSMC.Runtime;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage;

    private void Start()
    {
        PlayerManager player = PlayerManager.instance;

        if (/*!player.GetComponent<Collider2D>().isTrigger && */player.GetComponent<Collider2D>().CompareTag("Player") /*&& player.GetComponent<FSMC_Executer>().StateMachine.GetFloat("PlayerDistance") <= 8*/)
        {
            transform.position = new Vector3(player.objTransform.position.x + .5f, player.objTransform.position.y + 10, player.objTransform.position.z);
            player.TakeDamage(damage);
        }
    }
}
