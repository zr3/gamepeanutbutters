using Cinemachine;
using System.Collections;
using UnityEngine;

public class Juicer : MonoBehaviour
{
    [Header("Configuration")]
    public NoiseSettings CameraShakeProfile;
    public GameObject[] Fx;

    [Header("References")]
    public CinemachineVirtualCamera Camera;

    private static Juicer _instance;

    // camera shake
    private NoiseSettings initialProfile;
    private float initialAmplitudeGain;
    private float initialFrequencyGain;
    private CinemachineBasicMultiChannelPerlin noiseComponent;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        _instance = this.CheckSingleton(_instance);
        noiseComponent = Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public static void ShakeCamera(float intensity = 1f)
    {
        _instance.NonStaticShakeCamera(intensity);
    }

    private void NonStaticShakeCamera(float intensity)
    {
        if (shakeCoroutine == null)
        {
            initialProfile = noiseComponent.m_NoiseProfile;
            initialAmplitudeGain = noiseComponent.m_AmplitudeGain;
            initialFrequencyGain = noiseComponent.m_FrequencyGain;
        }
        else
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine());

        IEnumerator ShakeCoroutine()
        {
            noiseComponent.m_NoiseProfile = _instance.CameraShakeProfile;

            for (float i = 0.5f; i > 0; i -= 0.1f)
            {
                noiseComponent.m_AmplitudeGain = i * intensity;
                yield return new WaitForSeconds(0.1f);
            }

            noiseComponent.m_NoiseProfile = initialProfile;
            noiseComponent.m_AmplitudeGain = initialAmplitudeGain;
            noiseComponent.m_FrequencyGain = initialFrequencyGain;
        }
    }

    public static void CreateFx(int fxIndex, Vector3 position, Quaternion rotation)
    {
        _instance.NonStaticCreateFx(fxIndex, position, rotation);
    }

    public static void CreateFx(int fxIndex, Vector3 position)
    {
        _instance.NonStaticCreateFx(fxIndex, position, Quaternion.identity);
    }

    private void NonStaticCreateFx(int fxIndex, Vector3 position, Quaternion rotation)
    {
        var gob = GobPool.Instantiate(Fx[fxIndex]);
        gob.transform.position = position;
        gob.transform.rotation = rotation;
    }
}