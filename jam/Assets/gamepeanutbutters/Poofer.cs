using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Poofer : MonoBehaviour {

    public AudioClip[] AudioClips;
    public AudioMixerGroup SfxMixerGroup;
    public float Interval = 0.5f;
    private ParticleSystem ps;
    private AudioSource @as;
    private Vector3 lastPosition;
    private Coroutine playCoroutine;

	void Start () {
        ps = GetComponent<ParticleSystem>();
        if (AudioClips.Length > 0) @as = gameObject.AddComponent<AudioSource>();
        if (SfxMixerGroup && @as) @as.outputAudioMixerGroup = SfxMixerGroup;
        lastPosition = transform.position;
	}

    bool isMoving = false;
	void Update () {
        bool wasMoving = isMoving;
        isMoving = lastPosition != transform.position;

        if (isMoving && !wasMoving)
        {
            playCoroutine = StartCoroutine(PlayCoroutine());
        } else if (wasMoving && !isMoving)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
        lastPosition = transform.position;
    }

    IEnumerator PlayCoroutine()
    {
        infinite:
            TryPlayParticles();
            TryPlaySound();
            yield return new WaitForSeconds(Interval);
        goto infinite;
    }

    private void TryPlayParticles()
    {
        if (ps)
        {
            ps.Play();
        }
    }

    private void TryPlaySound()
    {
        if (@as)
        {
            @as.PlayOneShot(AudioClips[Random.Range(0, AudioClips.Length)]);
        }
    }
}
