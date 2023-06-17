using System;
using System.Reflection;
using TMPro;
using UnityEngine;

public class CDSkillObject : MonoBehaviour
{
    public SkillCDLink skills;
    public int number;
    public float CD;
    MonoBehaviour mono;
    public MonoBehaviour monoStep;
    int num;
    public FieldInfo valueFieldStep;
    // Start is called before the first frame update
    void Start()
    {
        if (number >= 0 && number <= skills.valuesList.Count)
        {
            foreach (MonoBehaviour script in skills.scripts)
            {
                if (script != this)
                {
                    Type scriptType = script.GetType();

                    FieldInfo valueField = scriptType.GetField("skillCD", BindingFlags.Public | BindingFlags.Instance);
                    if (valueField != null)
                    {
                        num = script.GetComponent<CDSkills>().number;
                        if (num == number)
                        {
                            mono = script;
                            mono.GetComponent<CDSkills>().skillCDMax = CD;
                            break;
                        }
                    }
                }
            }
            SetCD();
        }
    }

    public void SetCD()
    {
        foreach (MonoBehaviour script in skills.scripts)
        {
            if (script != this)
            {
                Type scriptType = script.GetType();

                valueFieldStep = scriptType.GetField("stepMax", BindingFlags.Public | BindingFlags.Instance);
                if (valueFieldStep != null)
                {
                    if (num == number && gameObject.name == script.gameObject.name)
                    {
                        monoStep = script;
                        valueFieldStep.SetValue(monoStep, CD);
                        break;
                    }
                }
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (number >= 0 && number <= skills.valuesList.Count)
        {
            foreach (MonoBehaviour script in skills.scripts)
            {
                if (script != this)
                {

                    Type scriptType = script.GetType();

                    valueFieldStep = scriptType.GetField("step", BindingFlags.Public | BindingFlags.Instance);

                    if (valueFieldStep != null)
                    {
                        if (num == number && gameObject.name == script.gameObject.name)
                        {
                            mono.GetComponent<CDSkills>().skillCD = (float)valueFieldStep.GetValue(monoStep);
                            mono.GetComponentInChildren<CDSkills>().text.GetComponent<TextMeshProUGUI>().text = mono.GetComponent<CDSkills>().skillCD.ToString("0");
                            if (mono.GetComponent<CDSkills>().skillCD <= 0)
                            {
                                mono.GetComponentInChildren<CDSkills>().text.SetActive(false);
                            }
                            else
                            {
                                mono.GetComponentInChildren<CDSkills>().text.SetActive(true);
                            }
                            break;
                        }

                    }
                }
            }
        }
    }
}
