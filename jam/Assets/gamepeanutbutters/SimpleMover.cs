using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMover : MonoBehaviour {
    public Vector3 RotationSpeed;
    public Vector3 RotationSineAmplitude;
    public Vector3 RotationSineFrequency;
    public Vector3 RotationSineOffset;

    public Vector3 TranslationSpeed;
    public Vector3 TranslationSineAmplitude;
    public Vector3 TranslationSineFrequency;
    public Vector3 TranslationSineOffset;

    private Vector3 deltaTranslation;
    private Quaternion deltaRotation;

    private new Transform transform;

    private void Awake()
    {
        transform = gameObject.transform;
    }

    private void CalculateRotationDelta()
    {
        deltaRotation =
            Quaternion.Euler(RotationSpeed * Time.deltaTime)
            * Quaternion.Euler(
                RotationSineAmplitude.x > 0 ? Mathf.Sin(Time.time * RotationSineFrequency.x + RotationSineOffset.x) * RotationSineAmplitude.x * Time.deltaTime : 0,
                RotationSineAmplitude.y > 0 ? Mathf.Sin(Time.time * RotationSineFrequency.y + RotationSineOffset.y) * RotationSineAmplitude.y * Time.deltaTime : 0,
                RotationSineAmplitude.z > 0 ? Mathf.Sin(Time.time * RotationSineFrequency.z + RotationSineOffset.z) * RotationSineAmplitude.z * Time.deltaTime : 0
            );
    }

    private void CalculateTranslationDelta()
    {
        deltaTranslation = TranslationSpeed * Time.deltaTime
            + new Vector3(
                TranslationSineAmplitude.x > 0 ? Mathf.Sin(Time.time * TranslationSineFrequency.x + TranslationSineOffset.x) * TranslationSineAmplitude.x * Time.deltaTime : 0,
                TranslationSineAmplitude.y > 0 ? Mathf.Sin(Time.time * TranslationSineFrequency.y + TranslationSineOffset.y) * TranslationSineAmplitude.y * Time.deltaTime : 0,
                TranslationSineAmplitude.z > 0 ? Mathf.Sin(Time.time * TranslationSineFrequency.z + TranslationSineOffset.z) * TranslationSineAmplitude.z * Time.deltaTime : 0
            );
    }

    private void ApplyDeltas()
    {
        transform.rotation *= deltaRotation;
        transform.position += deltaTranslation;
    }

    void Update () {
        CalculateRotationDelta();
        CalculateTranslationDelta();
        ApplyDeltas();
    }
}
