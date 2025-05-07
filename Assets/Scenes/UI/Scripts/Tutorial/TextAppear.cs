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

    // Ініціалізуємо таймер
    public int timer = 0;

    // Ініціалізуємо флаг, який вказує, чи зупинено появу тексту
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
        // Запускаємо таймер
        StartCoroutine(DisplayText());
    }
    IEnumerator DisplayText()
    {
        while (true)
        {
            if (!isPaused)
            {
                string currentText = introDialog[tutor.phase].Text[textCount];
                textMesh.text = "";

                for (int i = 0; i < currentText.Length; i++)
                {
                    textMesh.text += currentText[i];

                    if (currentText[i] == '.' && i > 1 && currentText[i - 1] == '.' && currentText[i - 2] == '.')
                    {
                        yield return new WaitForSeconds(delayDots);
                    }
                    else
                    {
                        yield return new WaitForSeconds(delay);
                    }

                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        textMesh.text = currentText;
                        break;
                    }
                }

                isPaused = true;
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0) && isPaused)
            {
                Resume();
            }

            yield return null;
        }
    }

    // Метод для відновлення появи тексту
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
                switch (tutor.phase)
                {
                    case 0:
                        tutor.parentPhase1.gameObject.SetActive(true);
                        break;
                    case 1:
                        mob.SetActive(true);
                        mob.AddComponent<EnemyHealthTutorial>();
                        CutsceneManager.Instance.StartCutscene("EnemyShow");
                        break;
                    case 2:
                        mob.GetComponent<EnemyHealthTutorial>().isCutScene = false;
                        tutor.MoveOn();
                        tutor.parentPhase2.gameObject.SetActive(true);
                        break;
                    case 3:
                        tutor.parentPhase3.gameObject.SetActive(true);
                        tutor.skillObject.SetActive(true);
                        tutor.MoveOn();
                        break;
                    case 4:
                        CutsceneManager.Instance.StartCutscene("BossShow");
                        boss.SetActive(true);
                        break;
                    case 5:
                        boss.GetComponent<BossAttack>().enabled = true;
                        boss.GetComponent<EnemyHealthTutorial>().isCutScene = false;
                        boss.GetComponent<FSMC_Executer>().UnMuteSpeed();
                        boss.GetComponent<FSMC_Executer>().SetCurrentState("Chase");
                        boss.GetComponent<AIDestinationSetter>().target = player.transform;
                        tutor.MoveOn();
                        break;
                    case 6:
                        tutor.MoveOn();
                        break;
                    case 7:
                        tutor.text.isShooting = true;
                        FinalPanel.SetActive(true);
                        break;
                }
            }
        }

    }
}
