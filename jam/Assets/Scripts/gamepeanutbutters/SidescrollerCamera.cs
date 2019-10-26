using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidescrollerCamera : MonoBehaviour {

    public Transform Target;
    public Vector2 Range;
    public float SmoothTime = 0.4f;
    public float MovementBump = 4;
    public float BumpTolerance = 0.1f;
    public float BumpTime = 1.5f;
    public Vector3 offset;

    private float vX = 0;
    private Vector3 targetPosition;
    private Vector3 lastTargetPosition;
    private Vector3 cameraTargetPosition;
    private float bumpTime = 0;

	void LateUpdate () {
        if (!Target)
        {
            return;
        }
        lastTargetPosition = targetPosition;
        targetPosition = Target.transform.position;
        bool bump = targetPosition.x - lastTargetPosition.x > BumpTolerance;
        bumpTime = bump ? bumpTime + Time.deltaTime : 0;
        bump = bumpTime > BumpTime;
        if (Mathf.Abs(transform.position.x - Target.transform.position.x) > Range.x - (bump ? MovementBump : 0))
        {
            cameraTargetPosition = bump ? targetPosition + Vector3.right * MovementBump : targetPosition;
        }
        transform.position = new Vector3(
            Mathf.SmoothDamp(transform.position.x + offset.x, cameraTargetPosition.x, ref vX, SmoothTime),
            transform.position.y,
            transform.position.z);
    }
}
