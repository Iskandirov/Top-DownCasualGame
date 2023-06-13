using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SkillCDLink : MonoBehaviour
{
    public List<float> valuesList = new List<float>();
    public MonoBehaviour[] scripts;

    private void Start()
    {
        scripts = FindObjectsOfType<MonoBehaviour>();
    }

    public void Check()
    {
        scripts = FindObjectsOfType<MonoBehaviour>();
    }

    private void FixedUpdate()
    {
    }
}
