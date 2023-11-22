using System.Collections;
using TMPro;
using UnityEngine;
[System.Serializable]
public class DialogPhase
{
    public string[] Text;
}
public class TextAppear : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public DialogPhase[] introDialog;
    public int textCount;
    public float delay = 1;
    public float delayDots = 1;

    // ���������� ������
    public int timer = 0;

    // ���������� ����, ���� �����, �� �������� ����� ������
    public bool isPaused = false;
    private bool isPausedByDots = false;
    public bool isShooting = false;
    public bool isPausedByAction = false;

    public Animator anim;
    public Tutor tutor;
    public GameObject boss;
    public GameObject mob;
    public GameObject FinalPanel;
    void Start()
    {
        // ��������� ������
        InvokeRepeating("AddLetter", delay, delay);
    }
    void AddLetter()
    {
        //Debug.Log(introDialog[tutor.phase - 1].Text[textCount][timer % introDialog[tutor.phase - 1].Text[textCount].Length]);
        // ���� ����� ������ �� ��������
        if (!isPaused)
        {
            // ���� ������� ����� - �� ������
            if (introDialog[tutor.phase].Text[textCount][timer % introDialog[tutor.phase].Text[textCount].Length] == '.')
            {
                // ���� ��������� ����� - �� ������
                if (timer > 0 && introDialog[tutor.phase].Text[textCount][timer - 1 % introDialog[tutor.phase].Text[textCount].Length] == '.')
                {
                    // ���� �������� ����� - �� ������
                    if (timer > 1 && introDialog[tutor.phase].Text[textCount][timer - 2 % introDialog[tutor.phase].Text[textCount].Length] == '.')
                    {
                        // ��������� ����� ������ �� 1 �������
                        StartCoroutine(PauseResume());
                    }
                }
            }
            textMesh.text += introDialog[tutor.phase].Text[textCount][timer % introDialog[tutor.phase].Text[textCount].Length];
            // ������������ ������
            timer++;
            if (textMesh.text.Length == introDialog[tutor.phase].Text[textCount].Length)
            {
                if (textCount < introDialog[tutor.phase].Text.Length - 1)
                {
                    Pause();
                    timer = 0;
                }
            }
        }
    }
    public IEnumerator PauseResume()
    {
        isPaused = true;
        isPausedByDots = true;
        yield return new WaitForSeconds(delayDots);
        isPaused = false;
        isPausedByDots = false;
    }
    // ����� ��� ������� ����� ������
    public void Pause()
    {
        isPaused = true;
    }
   
    // ����� ��� ���������� ����� ������
    public void Resume()
    {
        if (isPaused && !isPausedByDots && !isShooting && !isPausedByAction)
        {
            textCount++;

            if (textCount < introDialog[tutor.phase].Text.Length - 1)
            {
                textMesh.text = " ";
                isPaused = false;
            }
            else if(textCount == introDialog[tutor.phase].Text.Length - 1)
            {
                isPausedByAction = true;
                isPaused = true;
                anim.SetBool("FadeOut", true);
                textMesh.text = " ";
                textCount = 0;
                if (tutor.phase == 0)
                {
                    tutor.parentPhase1.gameObject.SetActive(true);
                }
                else if (tutor.phase == 1)
                {
                    mob.SetActive(true);
                    CutsceneManager.Instance.StartCutscene("EnemyShow");
                }
                else if (tutor.phase == 2)
                {
                    tutor.playerMove.enabled = true;
                    tutor.playerMove.GetComponent<Shoot>().enabled = true;
                    tutor.parentPhase2.gameObject.SetActive(true);
                    isShooting = true;
                } 
                else if (tutor.phase == 3)
                {
                    tutor.playerMove.enabled = true;
                    tutor.playerMove.GetComponent<Shoot>().enabled = true;
                    tutor.parentPhase3.gameObject.SetActive(true);
                    isShooting = true;
                    tutor.playerMove.isTutor = false;
                }
                else if (tutor.phase == 4)
                {
                    CutsceneManager.Instance.StartCutscene("BossShow");
                    boss.SetActive(true);
                } 
                else if (tutor.phase == 5)
                {
                    boss.GetComponent<Attack>().enabled = true;
                    boss.GetComponent<BossAttack>().enabled = true;
                    boss.GetComponent<Forward>().enabled = true;
                    boss.layer = 10;
                    tutor.playerMove.enabled = true;
                    tutor.playerMove.GetComponent<Shoot>().enabled = true;
                }
                else if (tutor.phase == 6)
                {
                    tutor.playerMove.enabled = true;
                }
                else if (tutor.phase == 7)
                {
                    FinalPanel.SetActive(true);
                }
            }
        }

    }
}
