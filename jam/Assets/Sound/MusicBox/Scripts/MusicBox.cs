using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Playables;

public class MusicBox : MonoBehaviour {
    public static MusicClip CurrentClip { get; private set; }
    public static MusicClip.MusicLocation MusicLocation => Instance.activeMusicChannel?.MusicLocation;

    [Header("Configuration")]
    [SerializeField]
    [Tooltip("MusicClips to make available for play.")]
    private MusicClip[] MusicClips;
    [SerializeField]
    [Tooltip("AudioClips to make available for ambient play.")]
    private AudioClip[] AmbientClips;

    [Header("Events")]
    [Tooltip("Fired immediately when a new clip is scheduled to play.")]
    public UnityEvent OnClipScheduled;
    [Tooltip("Fired when a clip is starts playing for the first time.")]
    public UnityEvent OnClipStart;
    [Tooltip("Fired when a clip loops back to the end of its Intro (which will often be the beginning of the clip).")]
    public UnityEvent OnClipLooped;
    [Tooltip("Fired when a clip plays its first beat.")]
    public UnityEvent OnFirstBeat;
    [Tooltip("Fired when a clip reaches the beginning of a bar.")]
    public UnityEvent OnBar;
    [Tooltip("Fired when a clip reaches the beginning of a beat.")]
    public UnityEvent OnBeat;

    [Header("References")]
    [Tooltip("Set to the music mixer group inside the main mixer.")]
    [SerializeField]
    private AudioMixerGroup musicMixerGroup;

    private Stack<(AudioSource, AudioPlayableOutput)> unusedAudioSourcePool = new Stack<(AudioSource, AudioPlayableOutput)>();
    private PlayableGraph playableGraph;
    private MusicChannel activeMusicChannel;

    public static MusicBox Instance { get; private set; }

    void Awake()
    {
        Instance = this.CheckSingleton(Instance);
        if (!MusicClips.Any())
        {
            return;
        }

        playableGraph = PlayableGraph.Create();
    }

    void OnDisable()
    {
        playableGraph.Destroy();
    }

    /// <summary>
    /// Schedules a transition to a new music clip. The configured Beginning of the new clip will line up with the nearest configured Ending of the currently playing clip, if one exists.
    /// </summary>
    /// <param name="index">Index of music clip configured in the MusicBox component.</param>
    public static void ChangeMusic(int index)
    {
        Instance._ChangeMusic(index);
    }

    private int currentClipIndex;

    private bool CheckCurrentClip(int index)
    {
        return index == currentClipIndex;
    }

    private void _ChangeMusic(int index)
    {
        var delay = activeMusicChannel == null ? 0 : activeMusicChannel.Stop();

        var musicChannelPlayable = ScriptPlayable<MusicChannel>.Create(playableGraph);
        activeMusicChannel = musicChannelPlayable.GetBehaviour();
        CurrentClip = MusicClips[index];
        currentClipIndex = index;
        // only the latest scheduled clips will fire events
        // order of events when simultaneous is loop -> bar -> beat
        activeMusicChannel.OnLoop = () => {
            if (CheckCurrentClip(index))
            {
                Debug.Log($"Looped {CurrentClip.Name}");
                OnClipLooped.Invoke();
            }
        };
        activeMusicChannel.OnFirstBeat = () =>
        {
            Debug.Log($"First beat {CurrentClip.Name}");
            OnFirstBeat.Invoke();
        };
        activeMusicChannel.OnBar = () => {
            if (CheckCurrentClip(index))
            {
                Debug.Log($"Bar {CurrentClip.Name}");
                OnBar.Invoke();
            }
        };
        activeMusicChannel.OnBeat = () => {
            if (CheckCurrentClip(index))
            {
                Debug.Log($"Beat {CurrentClip.Name}");
                OnBeat.Invoke();
            }
        };
        activeMusicChannel.Load(CurrentClip, musicChannelPlayable, playableGraph);
        activeMusicChannel.Play(delay);

        var audioSource = PooledAudioSource;
        var playableOutput = audioSource.Item2;
        playableOutput.SetSourcePlayable(musicChannelPlayable, 0);

        activeMusicChannel.OnFinished = () =>
        {
            PoolAudioSource(audioSource);
            playableGraph.Disconnect(musicChannelPlayable, 0);

        };

        if (!playableGraph.IsPlaying()) playableGraph.Play();
    }

    /// <summary>
    /// Plays a permanently looping ambient audio clip.
    /// </summary>
    /// <param name="index">Index of audio clip configured in the MusicBox component.</param>
    public static void PlayAmbience(int index)
    {
        Instance._PlayAmbience(index);
    }

    private void _PlayAmbience(int index)
    {
        var audioSource = CreateAudioSource();
        audioSource.loop = true;
        audioSource.clip = AmbientClips[index];
        audioSource.Play();
    }

    private AudioSource CreateAudioSource()
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = musicMixerGroup;
        audioSource.spatialize = false;
        audioSource.playOnAwake = false;
        return audioSource;
    }

    private (AudioSource, AudioPlayableOutput) PooledAudioSource
    {
        get
        {
            if (unusedAudioSourcePool.Count > 0)
            {
                return unusedAudioSourcePool.Pop();
            } else
            {
                var audioSource = CreateAudioSource();
                var playableOutput = AudioPlayableOutput.Create(playableGraph, "MusicOut", audioSource);
                playableOutput.SetSourceOutputPort(0);
                return (audioSource, playableOutput);
            }
        }
    }

    private void PoolAudioSource((AudioSource, AudioPlayableOutput) value)
    {
        unusedAudioSourcePool.Push(value);
    }
}
