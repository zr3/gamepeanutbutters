using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class MusicBox : MonoBehaviour {
    public static MusicClip CurrentClip { get; private set; }
    public static MusicClip NextClip { get; private set; }
    public static int CurrentBeat => _instance.currentBeat;
    public static bool UsingChannelA => _instance.usingChannelA;

    [Header("Configuration")]
    public MusicClip[] MusicClips;

    [Header("Events")]
    public UnityEvent OnClipScheduled;
    public UnityEvent OnClipStart;
    public UnityEvent OnClipLooped;
    public UnityEvent OnBarStart;
    public UnityEvent OnBeat;

    [Header("References")]
    [Tooltip("Set to the top-level mixer in the project.")]
    [SerializeField]
    private AudioMixer mixer;
    [Tooltip("Set to the music mixer in the project.")]
    [SerializeField]
    private AudioMixer musicMixer;

    [Tooltip("Set to the music mixer group inside the main mixer.")]
    [SerializeField]
    private AudioMixerGroup musicMixerGroup;
    [Tooltip("Set to the A channel mixer group inside the music mixer.")]
    [SerializeField]
    private AudioMixerGroup channelAMixerGroup;
    [Tooltip("Set to the B channel mixer group inside the music mixer.")]
    [SerializeField]
    private AudioMixerGroup channelBMixerGroup;

    private bool usingChannelA = true;
    private Channel channelA;
    private Channel channelB;

    private static MusicBox _instance;

    private void Awake()
    {
        _instance = this.CheckSingleton(_instance);
    }

    void Start ()
    {
        if (!MusicClips.Any())
        {
            return;
        }
        musicMixer.outputAudioMixerGroup = musicMixerGroup;
        channelA = new Channel(this, channelAMixerGroup);
        channelB = new Channel(this, channelBMixerGroup);
        channelA.LoadClip(MusicClips[0]);
        channelA.PlayAt(3);
    }

    private int currentBeat = 0;
    private int numBeats = 0;
    private double nextBeatTime = AudioSettings.dspTime;

    void Update()
    {
        if (AudioSettings.dspTime > nextBeatTime && CurrentClip != null)
        {
            currentBeat = (currentBeat + 1) % numBeats;
            OnBeat.Invoke();
            if (currentBeat % CurrentClip.BeatsPerBar == 0)
            {
                OnBarStart.Invoke();
            }
            nextBeatTime += 60f / MusicBox.CurrentClip.BPM;
        }
    }

    public static void ChangeMusic(int index)
    {
        _instance.changeMusic(index);
    }

    private void changeMusic(int index)
    {
        double switchTime;
        MusicClip currentClip;
        MusicClip nextClip = MusicClips[index];
        if (usingChannelA)
        {
            currentClip = channelA.MusicClip;
            channelB.LoadClip(nextClip);
            switchTime = channelA.Stop();
            channelB.PlayAt(switchTime);
        } else
        {
            currentClip = channelB.MusicClip;
            channelA.LoadClip(nextClip);
            switchTime = channelB.Stop();
            channelA.PlayAt(switchTime);
        }
        usingChannelA = !usingChannelA;
        CurrentClip = currentClip;
        NextClip = nextClip;
        IEnumerator updateClipReferences()
        {
            yield return new WaitForSeconds(Convert.ToSingle(switchTime - AudioSettings.dspTime));
            CurrentClip = nextClip;
            NextClip = null;
        }
        StartCoroutine(updateClipReferences());
    }

    private class Channel
    {
        private MusicBox parent;
        private AudioMixerGroup audioMixerGroup;
        private List<AudioSource> audioSources = new List<AudioSource>();
        public MusicClip MusicClip { get; private set; }

        public Channel(MusicBox parent, AudioMixerGroup audioMixerGroup)
        {
            this.parent = parent;
            this.audioMixerGroup = audioMixerGroup;
        }

        public void LoadClip(MusicClip clip)
        {
            MusicClip = clip;
            // ensure there are enough audiosources for the music clip
            while (audioSources.Count < clip.ClipLayers.Length)
            {
                var audioSource = parent.gameObject.AddComponent<AudioSource>();
                audioSources.Add(audioSource);
                audioSource.loop = false;
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                audioSource.playOnAwake = false;
            }
            // set all audiosources to the correct audio clips, or null if it's an extra audiosource
            for (int i = 0; i < audioSources.Count; ++i)
            {
                audioSources[i].Stop(); // for safety
                audioSources[i].clip = clip.ClipLayers.Length > i
                    ? clip.ClipLayers[i].Clip
                    : null;
            }
            // notify
            parent.OnClipScheduled.Invoke();
        }

        public void PlayAt(double time)
        {
            if (loopCoroutine != null)
            {
                parent.StopCoroutine(loopCoroutine);
            }
            loopCoroutine = parent.StartCoroutine(ScheduleLoop(time + MusicClip.LoopLength));
            double playTime = time - MusicClip.Beginning.InSeconds(MusicClip.BeatsPerBar, MusicClip.BPM);
            foreach (var audioSource in audioSources)
            {
                audioSource.PlayScheduled(playTime);
            }
            // notify
            IEnumerator waitThenNotify()
            {
                yield return new WaitForSeconds(Convert.ToSingle(playTime - AudioSettings.dspTime));
                parent.OnClipStart.Invoke();
                parent.numBeats = MusicClip.Endings.Last().InBeats(MusicClip.BeatsPerBar) - MusicClip.Beginning.InBeats(MusicClip.BeatsPerBar);
                parent.currentBeat = 0; // TODO: adjust for beginning
                parent.nextBeatTime = AudioSettings.dspTime + 60f / MusicClip.BPM;
            }
            parent.StartCoroutine(waitThenNotify());
        }

        public double Stop()
        {
            if (MusicClip)
            {
                double stopTime = MusicClip.Endings
                    .Select(ending => ending.InSeconds(MusicClip.BeatsPerBar, MusicClip.BPM))
                    .First(endingTime => endingTime > audioSources[0].time)
                    - audioSources[0].time
                    + AudioSettings.dspTime;
                parent.StartCoroutine(ScheduleStop(stopTime));
                return stopTime;
            } else
            {
                return AudioSettings.dspTime;
            }
        }

        private Coroutine loopCoroutine;

        private IEnumerator ScheduleLoop(double loopbackTime)
        {
            double loopLength = MusicClip.LoopLength;
            float cycleWait = Convert.ToSingle(loopLength);
            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                foreach (var audioSource in audioSources) audioSource.PlayScheduled(loopbackTime);
                IEnumerator waitThenNotify()
                {
                    yield return new WaitForSeconds(Convert.ToSingle(loopbackTime - AudioSettings.dspTime));
                    parent.OnClipLooped.Invoke();
                }
                parent.StartCoroutine(waitThenNotify());
                yield return new WaitForSeconds(cycleWait);
                loopbackTime += loopLength;
            }
        }
        private IEnumerator ScheduleStop(double stopTime)
        {
            if (loopCoroutine != null)
            {
                parent.StopCoroutine(loopCoroutine);
            }
            yield return new WaitForSeconds(Convert.ToSingle(stopTime - AudioSettings.dspTime));
            foreach (var audioSource in audioSources) audioSource.Stop();
        }
    }
}
