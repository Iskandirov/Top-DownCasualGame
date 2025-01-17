using System.Collections;
using UnityEngine;
using FSMC.Runtime;
using System;
using Pathfinding;

[Serializable]
public class StunState : FSMC_Behaviour
{
    public GameObject stunObj;
    GameObject obj;
    AIPath path;
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        path = executer.GetComponent<AIPath>();
        obj = UnityEngine.Object.Instantiate(stunObj, executer.transform.position + new Vector3(0, 6, 0), Quaternion.identity, executer.transform);
        obj.SetActive(false);
    }
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        obj.SetActive(true);
        path.maxSpeed = 0;
        executer.anim.SetTrigger("Stun");
        executer.StartCoroutine(StunTime(stateMachine, executer));
    }

    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
    }

    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        obj.SetActive(false);
        executer.anim.SetTrigger("Chase");
    }
    IEnumerator StunTime(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        yield return new WaitForSeconds(stateMachine.GetFloat("Stun Time"));
        stateMachine.SetCurrentState("Chase", executer);
    }
}