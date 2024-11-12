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
        Debug.Log("Slow");
    }

    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
    
    }
    IEnumerator SlowTime(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        yield return new WaitForSeconds(stateMachine.GetFloat("SlowTime"));
        stateMachine.SetCurrentState("Chase", executer);
    }
}