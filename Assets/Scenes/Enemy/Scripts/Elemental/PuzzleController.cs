using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    private ElementalBoss_Attack bossAttack;
    private int targetStageIndex;

    private bool isSolved = false;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;


    int counter = 6;
    public void Init(ElementalBoss_Attack bossAttack, int targetStageIndex)
    {
        virtualCamera = GameObject.FindGameObjectWithTag("AdditionalCamera").GetComponent<CinemachineVirtualCamera>();

        this.bossAttack = bossAttack;
        this.targetStageIndex = targetStageIndex;
        
        MoveCameraToPuzzle();

    }

    private void MoveCameraToPuzzle()
    {
        if (virtualCamera == null) return;

        // кажемо камері слідкувати за цією головоломкою
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;
    }


    public void SolvePuzzle()
    {
        if (isSolved) return;
        isSolved = true;

        if (bossAttack != null)
        {
            bossAttack.OnPuzzleCompleted(targetStageIndex);
        }
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;
        Destroy(gameObject);
    }
    public void SigilsCounter()
    {
        counter--;
        if (counter <= 0)
        {
            SolvePuzzle();
        }
    }
}
