using UnityEngine;
using UnityEngine.Events;

public class CollisionEventBinding : MonoBehaviour
{
    public UnityEvent CollisionEnter;
    public UnityEvent CollisionExit;
    public UnityEvent TriggerEnter;
    public UnityEvent TriggerExit;

    public void OnCollisionEnter(Collision col)
    {
        CollisionEnter.Invoke();
    }

    public void OnCollisionExit(Collision col)
    {
        CollisionExit.Invoke();
    }

    public void OnTriggerEnter(Collider other)
    {
        TriggerEnter.Invoke();
    }

    public void OnTriggerExit(Collider other)
    {
        TriggerExit.Invoke();
    }
}
