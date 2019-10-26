using Cinemachine;
using UnityEngine;

public class CinemachineCameraShake : CinemachineExtension
{
    public float Blend;
    public Vector3 Amplitude;
    public Vector3 Frequency;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body && Blend > 0)
        {
            state.PositionCorrection += (
                Mathf.Sin(Time.time * Frequency.x) * Amplitude.x * VirtualCamera.VirtualCameraGameObject.transform.right
                + Mathf.Sin(Time.time * Frequency.y) * Amplitude.y * VirtualCamera.VirtualCameraGameObject.transform.up
                + Mathf.Sin(Time.time * Frequency.z) * Amplitude.z * VirtualCamera.VirtualCameraGameObject.transform.forward
            ) * Blend;
        }
    }
}
