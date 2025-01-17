using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMC.Runtime;
using System;
using static UnityEngine.EventSystems.EventTrigger;

[Serializable]
public class StateHit : FSMC_Behaviour
{
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        executer.health = executer.healthMax;
        if (executer.isBoss)
            executer.bossHealthObj.SetActive(true);
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