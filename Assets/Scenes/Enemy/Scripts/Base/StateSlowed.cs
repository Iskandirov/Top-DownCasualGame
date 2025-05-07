using System.Collections;
using UnityEngine;
using FSMC.Runtime;
using System;
using Pathfinding;

[Serializable]
public class StateSlowed : FSMC_Behaviour
{
    AIPath path;
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        path = executer.GetComponent<AIPath>();
    }
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        path.maxSpeed = executer.speed * stateMachine.GetFloat("SlowPercent");
        executer.StartCoroutine(SlowTime(stateMachine, executer));
    }

    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
    }

    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        executer.anim.SetBool("Chase", true);
    }
    IEnumerator SlowTime(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        yield return new WaitForSeconds(stateMachine.GetFloat("SlowTime"));
        stateMachine.SetCurrentState("Chase", executer);
    }
}