using UnityEngine;

public class PersistantGameState : ScriptableObject
{
    public string StateTitle { get; set; }
    public int HP { get; set; }
    public float ScaledHP { get; set; }
}
