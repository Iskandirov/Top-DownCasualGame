using FSMC.Runtime;
using Pathfinding;
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
    public int textCount = 0;
    public float delay = 1;
    public float delayDots = 1;

    // ≤н≥ц≥ал≥зуЇмо таймер
    public int timer = 0;

    // ≤н≥ц≥ал≥зуЇмо флаг, €кий вказуЇ, чи зупинено по€ву тексту
    public bool isPaused = false;
    private bool isPausedByDots = false;
    public bool isShooting = false;
    public bool isPausedByAction = false;

    public Animator anim;
    public Tutor tutor;
    public GameObject boss;
    public GameObject mob;
    public GameObject FinalPanel;
    PlayerManager player;
    void Start()
    {
        player = PlayerManager.instance;
        //Debug.Log(0 == introDialog.Length);
        for (int i = 0; i < introDialog.Length; i++)
        {
            for (int y = 0; y < introDialog[i].Text.Length - 1; y++)
            {
                introDialog[i].Text[y] = GameManager.Instance.localizedText.Find(p => p.key == "phrase_" + (i + 1) + "_" + (y + 1) && p.language == GameManager.Instance.loc).value;
            }
        }
        // «апускаЇмо таймер
        InvokeRepeating("AddLetter", delay, delay);
    }
    void AddLetter()
    {
        // якщо по€ву тексту не зупинено
        if (!isPaused)
        {
            // якщо поточна л≥тера - це крапка
            if (introDialog[tutor.phase].Text[textCount][timer % introDialog[tutor.phase].Text[textCount].Length] == '.')
            {
                // якщо попередн€ л≥тера - це крапка
                if (timer > 0 && introDialog[tutor.phase].Text[textCount][timer - 1 % introDialog[tutor.phase].Text[textCount].Length] == '.')
                {
                    // якщо наступна л≥тера - це крапка
                    if (timer > 1 && introDialog[tutor.phase].Text[textCount][timer - 2 % introDialog[tutor.phase].Text[textCount].Length] == '.')
                    {
                        // «атримуЇмо по€ву тексту на 1 секунду
                        StartCoroutine(PauseResume());
                    }
                }
            }
            textMesh.text += introDialog[tutor.phase].
                Text[textCount][timer % introDialog[tutor.phase].
                Text[textCount].
                Length];
            // ≤нкрементуЇмо таймер
            timer++;
            if (textMesh.text.Length == introDialog[tutor.phase].Text[textCount].Length)
            {
                if (textCount < introDialog[tutor.phase].Text.Length - 1)
                {
                    isPaused = true;
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
   
    // ћетод дл€ в≥дновленн€ по€ви тексту
    public void Resume()
    {
        if (isPaused && !isPausedByDots && !isShooting && !isPausedByAction)
        {
            textCount++;

            if (textCount < introDialog[tutor.phase].Text.Length - 1)
            {
                textMesh.text = " ";
                isPaused = false;
                tutor.BlockMoveAndShoot();
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
                    mob.AddComponent<EnemyHealthTutorial>();
                    CutsceneManager.Instance.StartCutscene("EnemyShow");
                }
                else if (tutor.phase == 2)
                {
                    mob.GetComponent<EnemyHealthTutorial>().isCutScene = false;

                    tutor.MoveOn();
                    tutor.parentPhase2.gameObject.SetActive(true);
                } 
                else if (tutor.phase == 3)
                {
                    tutor.parentPhase3.gameObject.SetActive(true);
                    tutor.skillObject.SetActive(true);
                    tutor.MoveOn();
                }
                else if (tutor.phase == 4)
                {
                    CutsceneManager.Instance.StartCutscene("BossShow");
                    boss.SetActive(true);
                } 
                else if (tutor.phase == 5)
                {
                    boss.GetComponent<BossAttack>().enabled = true;
                    boss.GetComponent<EnemyHealthTutorial>().isCutScene = false;
                    boss.GetComponent<FSMC_Executer>().UnMuteSpeed();
                    boss.GetComponent<FSMC_Executer>().SetCurrentState("Chase");
                    boss.GetComponent<AIDestinationSetter>().target = player.transform;
                    tutor.MoveOn();

                }
                else if (tutor.phase == 6)
                {
                    tutor.MoveOn();
                }
                else if (tutor.phase == 7)
                {
                    tutor.text.isShooting = true;
                    FinalPanel.SetActive(true);
                }
            }
        }

    }
}
