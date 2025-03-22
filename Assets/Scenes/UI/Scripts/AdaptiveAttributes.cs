using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[AttributeUsage(AttributeTargets.Field)]
public class AdaptiveAttributes : Attribute
{
    public string DisplayName;
    public AdaptiveAttributes(string displayName)
    {
        DisplayName = displayName;
    }
}
[System.Serializable]
public class PlayerData
{
    [AdaptiveAttributes("First stat")]
    private float Stat; 
    [AdaptiveAttributes("Second stat")]
    private float Stat2;
    public PlayerData()
    {
        Stat = 0;
        Stat2 = 0;
    }
    public void SetValue(float value)
    {
        Stat = value;
    }
    public float GetValue()
    {
        return Stat;
    }
    public void Update()
    {
        Stat += 1;
        //Stat2 += Time.deltaTime;
    }
}

public struct ExposedFieldInfo
{ 
    public MemberInfo memberInfo;
    public AdaptiveAttributes exposedFieldAttribute;
    public ExposedFieldInfo(MemberInfo info, AdaptiveAttributes attribute)
    {
        memberInfo = info;
        exposedFieldAttribute = attribute;
    }
}
//public class ExposedVariablesEditorwindow : EditorWindow
//{
//    List<ExposedFieldInfo> exposedMembers = new List<ExposedFieldInfo>();
//    [MenuItem("Tools/Exposed Variables")]
//    public static void ShowWindow()
//    {
//        ExposedVariablesEditorwindow window = GetWindow<ExposedVariablesEditorwindow>("Exposed Variables");
//    }
//    public void OnEnable()
//    {
//        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
//        foreach (Assembly assembly in assemblies)
//        {
//            Type[] types = assembly.GetTypes();
//            foreach (Type type in types)
//            {
//                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
//                MemberInfo[] members = type.GetMembers(flags);
//                foreach (MemberInfo member in members)
//                {
//                    if (member.CustomAttributes.ToArray().Length > 0)
//                    {
//                        AdaptiveAttributes attribute = member.GetCustomAttribute<AdaptiveAttributes>();
//                        if (attribute != null)
//                        {
//                            exposedMembers.Add(new ExposedFieldInfo(member, attribute));
//                        }
//                    }
//                }
//            }
//        }
//    }
    //public void OnGUI()
    //{

    //    EditorGUILayout.LabelField("Exposed Properties", EditorStyles.boldLabel);
    //    foreach (ExposedFieldInfo member in exposedMembers)
    //    {
    //        MemberInfo memberInfo = member.memberInfo;
    //        AdaptiveAttributes attribute = member.exposedFieldAttribute;

    //        if (memberInfo.MemberType == MemberTypes.Field)
    //        {
    //            FieldInfo fieldInfo = (FieldInfo)memberInfo;

    //            object obj = null;
    //            string value = "N/A";
    //            if (memberInfo.ReflectedType == GameManager.Instance.playerData.GetType())
    //            {
    //                obj = fieldInfo.GetValue(GameManager.Instance.playerData);
    //            }

    //            if (obj != null)
    //            {
    //                value = obj.ToString();
    //            }

    //            EditorGUILayout.LabelField($"{attribute.DisplayName}: {value}");
    //            //Debug.Log($"{attribute.DisplayName}: {value}");
    //        }
    //        else
    //        {
    //            EditorGUILayout.LabelField($"{member.exposedFieldAttribute.DisplayName}");
    //        }
    //    }
    //}
//}