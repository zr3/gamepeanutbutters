using UnityEngine;

public class CelLightHack : MonoBehaviour
{
    private const int nagEveryFrames = 60;
    private const string lightTag = "MainLight";
    private new Transform light;
    private int lightNag = 0;

    void Update()
    {
        if (!light)
        {
            lightNag--;
            light = GameObject.FindWithTag(lightTag)?.transform;
            if (lightNag <= 0 && !light)
            {
                lightNag = nagEveryFrames;
                Debug.LogError($"{nameof(CelLightHack)} on {gameObject.name} could not find the main light. Make sure there a gameobject with the tag {lightTag}.");
            }
        }
        if (light)
        {
            Shader.SetGlobalVector("_CelLightDirection", -light.forward);
        }
    }
}
