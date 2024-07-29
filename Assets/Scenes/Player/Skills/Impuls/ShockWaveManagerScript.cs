using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShockWaveManagerScript : MonoBehaviour
{
    [SerializeField] private float shockWaveTime = .75f;
    private Coroutine shockWaveCoroutine;
    private Material mat;
    private static int waveDistanceFromCenter = Shader.PropertyToID("_WaveDistance");
    // Start is called before the first frame update
    void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            CallShockWave();
        }
    }
    public void CallShockWave()
    {
        shockWaveCoroutine = StartCoroutine(ShockWaveAction(-0.1f, 1f));
    }
    private IEnumerator ShockWaveAction(float startPos,float endPos)
    {
        mat.SetFloat(waveDistanceFromCenter, startPos);
        float lerpedAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < shockWaveTime)
        {
            elapsedTime += Time.deltaTime;
            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / shockWaveTime));
            mat.SetFloat(waveDistanceFromCenter, lerpedAmount);
            yield return null;
        }
    }
}
