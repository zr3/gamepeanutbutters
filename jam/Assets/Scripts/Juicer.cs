using Cinemachine;
using System.Collections;
using UnityEngine;

public class Juicer : MonoBehaviour
{
    [Header("Configuration")]
    public NoiseSettings CameraShakeProfile;
    public GameObject[] Fx;

    [Header("References")]
    public CinemachineCameraShake Camera;

    private static Juicer _instance;

    // camera shake
    private NoiseSettings initialProfile;
    private float initialAmplitudeGain;
    private float initialFrequencyGain;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        _instance = this.CheckSingleton(_instance);
    }

    public static void ShakeCamera(float intensity = 1f)
    {
        _instance.NonStaticShakeCamera(intensity);
    }

    private void NonStaticShakeCamera(float intensity)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine());

        IEnumerator ShakeCoroutine()
        {
            for (float i = 0.5f; i > 0; i -= Time.deltaTime)
            {
                Camera.Blend = i * intensity * 2f;
                yield return null;
            }
            Camera.Blend = 0;
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