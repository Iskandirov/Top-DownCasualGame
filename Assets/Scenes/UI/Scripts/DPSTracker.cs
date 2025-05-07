using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DPSTracker : MonoBehaviour
{

    private static float dps;
    private static float totalDamage = 0f;
    private static float timer = 0f;
    public static float updateInterval = 1f;
    [SerializeField] TextMeshProUGUI dpsText;

    public static float CurrentDPS => dps;

    public static void RegisterDamage(float damage)
    {
        totalDamage += damage;
    }
    public static float GetTotalDamage()
    {
        return totalDamage;
    }
    void Update()
    {

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            dps = totalDamage / timer;
            dpsText.text = dps.ToString("F2") + " DPS";
            totalDamage = 0f;
            timer = 0f;
        }
    }
}
