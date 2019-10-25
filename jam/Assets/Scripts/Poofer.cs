using UnityEngine;

public class Poofer : MonoBehaviour {

    private ParticleSystem ps;
    private float lastMovement = 0.6f;
    private Vector3 lastPosition;

	void Start () {
        ps = GetComponentInChildren<ParticleSystem>();
        lastPosition = transform.position;
	}

	void Update () {
        lastMovement += Time.deltaTime;
		if (lastPosition != transform.position)
        {
            lastMovement = 0;
        }
        if (lastMovement > 0.2f && ps.isPlaying)
        {
            ps.Stop();
        } else if (lastMovement == 0 && !ps.isPlaying)
        {
            ps.Play();
        }
        lastPosition = transform.position;
	}
}
