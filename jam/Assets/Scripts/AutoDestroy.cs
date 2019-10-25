using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {
    public bool DestroyAfterTime = true;
    public float DestroyTimer = 5;

    public bool DestroyAfterParticlesFinished = false;

    public bool DestroyAfterSound = false;

    private bool timeFinished;
    private bool particlesFinished;
    private bool soundFinished;

    private void Awake()
    {
        timeFinished = !DestroyAfterTime;
        particlesFinished = !DestroyAfterParticlesFinished;
        soundFinished = !DestroyAfterSound;

        if (DestroyAfterTime)
        {
            IEnumerator destroyAfterTime()
            {
                yield return new WaitForSeconds(DestroyTimer);
                timeFinished = true;
                TryToKill();
            }
            StartCoroutine(destroyAfterTime());
        }

        if (DestroyAfterParticlesFinished)
        {
            var particleSystems = GetComponentsInChildren<ParticleSystem>();
            IEnumerator destroyAfterParticles()
            {
                while (!particleSystems.All(ps => ps.isStopped))
                {
                    yield return new WaitForSeconds(1);
                }
                particlesFinished = true;
                TryToKill();
            }
            StartCoroutine(destroyAfterParticles());
        }

        if (DestroyAfterSound)
        {
            var audioSources = GetComponentsInChildren<AudioSource>();
            IEnumerator destroyAfterSound()
            {
                while (audioSources.Any(@as => @as.isPlaying))
                {
                    yield return new WaitForSeconds(1);
                }
                soundFinished = true;
                TryToKill();
            }
            StartCoroutine(destroyAfterSound());
        }
    }

    private void TryToKill() {
        if (timeFinished && particlesFinished && soundFinished) GobPool.Destroy(gameObject);
	}
}
