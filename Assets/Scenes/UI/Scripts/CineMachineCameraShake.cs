using Cinemachine;
using UnityEngine;

public class CineMachineCameraShake : MonoBehaviour
{
    public static CineMachineCameraShake instance { get; private set; }
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float ShakeTime;
    private void Awake()
    {
        instance = this;
        Shake(0,0);
    }
    public void Shake(float intensity, float timer)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        ShakeTime = timer;
    }
    private void FixedUpdate()
    {
        if (ShakeTime > 0)
        {
            ShakeTime -= Time.deltaTime;
            if (ShakeTime <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                    virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }
}
