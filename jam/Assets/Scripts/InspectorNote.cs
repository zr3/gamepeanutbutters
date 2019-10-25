using UnityEngine;

public class InspectorNote : MonoBehaviour
{
    public string Message = "this is a note.";

    void Awake()
    {
        enabled = false;
    }
}
