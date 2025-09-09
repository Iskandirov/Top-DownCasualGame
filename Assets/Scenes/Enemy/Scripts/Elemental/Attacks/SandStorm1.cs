using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandStorm1 : MonoBehaviour
{
    [Header("SandStorm Settings")]
    [SerializeField] private GameObject sandstormPrefab;
    [SerializeField] private float sandstormDuration = 8f;
    [SerializeField] private float sandstormDamagePerSecond = 5f;
    [SerializeField] private float slowMultiplier = 0.5f;
    GameObject storm;
    SandStorm sand;
    private bool previousZoneState = false;
    private Coroutine distortionRoutine;
    public Material sandStormMat;
    public void ExecuteAttack()
    {
        StartCoroutine(SandstormRoutine());
    }
    IEnumerator SandstormRoutine()
    {
        storm = Instantiate(sandstormPrefab, transform.position, Quaternion.identity);
        sand = storm.GetComponent<SandStorm>();
        SandStorm zone = storm.GetComponent<SandStorm>();
        zone.Setup(sandstormDuration, sandstormDamagePerSecond, slowMultiplier);
        yield return new WaitForSeconds(sandstormDuration);
        distortionRoutine = StartCoroutine(DistortionManager(0f, 2f));
        sand = null;
        Destroy(storm);
    }
    private void FixedUpdate()
    {
        if (sand == null) return;

        if (sand.inZone != previousZoneState)
        {
            previousZoneState = sand.inZone;

            if (distortionRoutine != null)
                StopCoroutine(distortionRoutine);

            float target = sand.inZone ? 10f : 0f;
            distortionRoutine = StartCoroutine(DistortionManager(target, 2f));
        }
    }
    public IEnumerator DistortionManager(float targetValue, float duration)
    {

        if (sandStormMat == null) yield break;

        float startValue = sandStormMat.GetFloat("_VignetteIntensity");
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float newValue = Mathf.Lerp(startValue, targetValue, t);
            sandStormMat.SetFloat("_VignetteIntensity", newValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        sandStormMat.SetFloat("_VignetteIntensity", targetValue); // точне завершення
    }
}