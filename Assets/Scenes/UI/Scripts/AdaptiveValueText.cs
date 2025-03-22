using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextMeshProUGUI))]
public class AdaptiveValueText : MonoBehaviour
{
    private static Dictionary<string, FieldInfo> fieldNameDictionary;

    private TextMeshProUGUI textField;
    List<string> instancevalues = new List<string>();
    private object cachedPlayerData;

    [TextArea()]
    public string textValue;
    public AdaptiveValueSelector[] adaptiveValues;
    public void Awake()
    {
        textField = GetComponent<TextMeshProUGUI>();
        if (fieldNameDictionary == null)
        {
            CreateDictionary();
        }
        cachedPlayerData = GameManager.Instance.playerData;
    }
    public void UpdateValues()
    {
        textField.text = GetFormatedString();
    }
    public void OnEnable()
    {
        UpdateValues();
    }
    string GetFormatedString()
    {
        instancevalues.Clear();

        foreach (var adaptiveValue in adaptiveValues)
        {
            if (fieldNameDictionary.TryGetValue(adaptiveValue.fieldName, out FieldInfo field))
            {
                instancevalues.Add(GetValue(field));
            }
            else
            {
                instancevalues.Add("MISSING");
            }
        }
        return string.Format(textValue, instancevalues.ToArray());
    }
    string GetValue(FieldInfo field)
    {
        if (cachedPlayerData == null) return "MISSING"; // Запобігаємо NullReferenceException

        object obj = field.GetValue(cachedPlayerData);
        return obj != null ? obj.ToString() : "MISSING";
    }
    void CreateDictionary()
    {
        fieldNameDictionary = new Dictionary<string, FieldInfo>();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var attribute = field.GetCustomAttribute<AdaptiveAttributes>();
                    if (attribute != null)
                    {
                        fieldNameDictionary[field.Name] = field;
                    }
                }
            }
        }
    }
}
