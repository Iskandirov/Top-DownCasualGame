using System.Collections.Generic;
using UnityEngine;

public class SkillCDLink : MonoBehaviour
{
    public List<float> valuesList = new List<float>();
    public MonoBehaviour[] scripts;

    private void Start()
    {
        Check();
    }

    public void Check()
    {
        scripts = FindObjectsOfType<MonoBehaviour>();
    }
}
