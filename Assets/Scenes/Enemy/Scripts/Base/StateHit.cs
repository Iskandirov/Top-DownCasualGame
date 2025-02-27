using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMC.Runtime;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

[Serializable]
public class StateHit : FSMC_Behaviour
{
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        executer.health = executer.healthMax;
        if (executer.isBoss && !LootManager.inst.isTutor)
        {
            Transform parent = GameObject.Find("UI").transform;
            executer.expiriancePoint = GameObject.Find("Expiriance");
            GameObject instantiatedObject = UnityEngine.Object.Instantiate(executer.bossHealthObj, parent);
            executer.healthObjImg = instantiatedObject.transform.GetChild(1).GetComponent<Image>();
            executer.avatar = instantiatedObject.transform.GetChild(2).GetComponent<Image>();
            executer.bossHealthObj.SetActive(true);
            executer.avatar.sprite = GameManager.ExtractSpriteListFromTexture("boss_avatars").First(a => a.name == ("boss_avatars_" + (SceneManager.GetActiveScene().buildIndex - 2)));
            Debug.Log("boss_avatars_" + (SceneManager.GetActiveScene().buildIndex - 2));
        }
        else if(executer.isBoss && LootManager.inst.isTutor)
        {
            executer.bossHealthObj.SetActive(true);
        }
    }
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        if (executer.isBoss)
            executer.healthObjImg.fillAmount -= executer.GetDamage() / executer.healthMax;

        executer.anim.SetTrigger("Hit");
        if (executer.health <= 0)
        {
            stateMachine.SetCurrentState("Death", executer);
        }
        else if (executer.GetFloat("Stun Time") > 0.2f)
        {
            stateMachine.SetCurrentState("Stun", executer);
        }
        else if (executer.GetFloat("SlowTime") > 0.2f)
        {
            stateMachine.SetCurrentState("Slowed", executer);
        }
        else
        {
            stateMachine.SetCurrentState("Chase", executer);
        }
    }

    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
    }

    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {

    }
}