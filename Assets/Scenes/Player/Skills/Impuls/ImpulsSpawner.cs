using System.Collections;
using UnityEngine;

public class ImpulsSpawner : MonoBehaviour
{
    public Impuls impuls;
    public float step;
    public float stepMax;
    public float powerGrow;
    public bool isFour;
    public bool isFive;
    PlayerManager player;
    int buttonActivateSkill;
    KeyCode keyCode;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
        step = gameObject.GetComponent<CDSkillObject>().CD;
        StartCoroutine(SetBumberToSkill());
    }
    private IEnumerator SetBumberToSkill()
    {
        yield return new WaitForSeconds(0.1f);
        buttonActivateSkill = gameObject.GetComponent<CDSkillObject>().num + 1;
        keyCode = (KeyCode)((int)KeyCode.Alpha0 + buttonActivateSkill);
    }
    private void FixedUpdate()
    {
        step -= Time.deltaTime;
    }
    void Update()
    {
        if (step <= 0 && Input.GetKeyDown(keyCode))
        {
            Impuls a = Instantiate(impuls, transform.position, Quaternion.identity);
            a.powerGrow = powerGrow;
            a.isFour = isFour;
            a.isFive = isFive;
            a.wind = player.Wind;
            a.grass = player.Grass;
            step = stepMax;
        }
    }
}
