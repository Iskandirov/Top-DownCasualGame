using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
//[CustomPropertyDrawer(typeof(AdaptiveValueSelector))]
//public class AdaptiveValueSelectorPropertyDrawer : PropertyDrawer
//{
//    [SerializeField] List<FieldInfo> fields;
//    [SerializeField] List<string> fieldNames;
//    [SerializeField] bool gotFields;
//    int index;
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        if (!gotFields)
//        {
//            GetFields();
//        }
//        SerializedProperty fieldNameProperty = property.FindPropertyRelative("fieldName");

//        index = GetFieldName(fieldNameProperty.stringValue);
//        index = EditorGUI.Popup(position, index, fieldNames.ToArray());
//        fieldNameProperty.stringValue = fields[index].Name;
//    }
//    private void GetFields()
//    {
//        fields = new List<FieldInfo>();
//        fieldNames = new List<string>();

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
//                            fields.Add((FieldInfo)member);
//                            fieldNames.Add($"{member.ReflectedType}/{attribute.DisplayName}");
//                        }
//                    }
//                }
//            }
//        }
//        gotFields = true;
//    }
//    private int GetFieldName(string value)
//    {
//        string fieldName = value;
//        int count = 0;
//        foreach (FieldInfo member in fields)
//        {
//            if (member.Name == fieldName)
//            {
//                return count;
//            }
//            count++;
//        }
//        return 0;
//    }
//}