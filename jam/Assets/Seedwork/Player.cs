using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController characterController;
    void Update()
    {
        characterController.SimpleMove(transform.forward * Input.GetAxis("Vertical") * 5);
        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * 180f * Time.deltaTime);
    }
}
