using System;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class MusicChannel : PlayableBehaviour
{
    public MusicClip Clip { get; private set; }
    public double NextEnding(double leadTime) => Clip
            .Endings
            .First(e => e.InSeconds(Clip.BeatsPerBar, Clip.BPM) > currentTimeInClipSpace + leadTime)
            .InSeconds(Clip.BeatsPerBar, Clip.BPM);
    public MusicClip.MusicLocation MusicLocation => new MusicClip.MusicLocation
    {
        Beat = (nextBeat - 1) % Clip.BeatsPerBar + 1,
        Bar = (nextBeat - 1) / Clip.BeatsPerBar + 1
    };
    public Action OnFinished;
    public Action OnLoop;
    public Action OnFirstBeat;
    public Action OnBar;
    public Action OnBeat;

    private Playable mixer;
    private double clipLoopbackToTime, clipLoopbackFromTime, currentTimeInClipSpace, calculatedLoopTime;
    private AudioClipPlayable[] inputPlayables;
    private double endTime;

    public void Load(MusicClip musicClip, Playable owner, PlayableGraph graph)
    {
        Clip = musicClip;
        owner.SetInputCount(1);
        mixer = AudioMixerPlayable.Create(graph, musicClip.ClipLayers.Length);
        graph.Connect(mixer, 0, owner, 0);
        owner.SetInputWeight(0, 1);
        inputPlayables = new AudioClipPlayable[musicClip.ClipLayers.Length];
        for (int i = 0; i < musicClip.ClipLayers.Length; i++)
        {
            inputPlayables[i] = AudioClipPlayable.Create(graph, musicClip.ClipLayers[i].Clip, false);
            graph.Connect(inputPlayables[i], 0, mixer, i);
            mixer.SetInputWeight(i, 1f);
        }
    }

    private bool firstBeatEventSent = false;

    public void Play(double delay = 0)
    {
        for (int i = 0; i < inputPlayables.Length; i++)
        {
            inputPlayables[i].Seek(0, delay);
        }
        clipLoopbackToTime = Clip.IntroEnd.InSeconds(Clip.BeatsPerBar, Clip.BPM);
        clipLoopbackFromTime = Clip.VampEnd.InSeconds(Clip.BeatsPerBar, Clip.BPM);
        calculatedLoopTime = Clip.LoopLength;
        endTime = double.MaxValue;

        currentTimeInClipSpace = -delay;
        nextBeatTime = 0;
        beatLength = 60.0 / Clip.BPM;
        nextBeat = 0;
    }

    public double Stop()
    {
        endTime = Clip.Endings
            .Select(ending => ending.InSeconds(Clip.BeatsPerBar, Clip.BPM))
            .First(endingTime => endingTime > currentTimeInClipSpace);
        return TimeUntilEnd;
    }

    public double TimeUntilEnd => Clip.Endings
        .Select(ending => ending.InSeconds(Clip.BeatsPerBar, Clip.BPM))
        .First(endingTime => endingTime > currentTimeInClipSpace)
        - currentTimeInClipSpace;

    const double buffer = 0.05;

    private double nextBeatTime, beatLength;
    private int nextBeat;

    public override void PrepareFrame(Playable owner, FrameData info)
    {
        if (mixer.GetInputCount() == 0) return;
        currentTimeInClipSpace += info.deltaTime;

        // loop clips
        if (endTime == double.MaxValue)
        {
            double lookAhead = currentTimeInClipSpace + buffer;
            if (lookAhead >= clipLoopbackFromTime)
            {
                double offset = clipLoopbackFromTime - currentTimeInClipSpace;
                foreach (var audioClipPlayable in inputPlayables)
                {
                    audioClipPlayable.Seek(clipLoopbackToTime, offset);
                }
                currentTimeInClipSpace = clipLoopbackToTime - offset;
                nextBeatTime = clipLoopbackToTime;
                nextBeat = Clip.IntroEnd.InBeats(Clip.BeatsPerBar) - 1;
                OnLoop();
            }
        } else if (currentTimeInClipSpace > endTime && !owner.IsDone())
        {
            foreach (var audioClipPlayable in inputPlayables)
            {
                audioClipPlayable.Pause();
            }
            owner.SetDone(true);
            OnFinished();
        }

        // notify beats and bars
        if (currentTimeInClipSpace >= nextBeatTime)
        {
            onBeat();
            nextBeatTime += beatLength;
        }

        base.PrepareFrame(owner, info);
    }

    private void onBeat()
    {
        nextBeat++;
        if (!firstBeatEventSent)
        {
            OnFirstBeat();
            firstBeatEventSent = true;
        }
        if ((nextBeat - 1) % Clip.BeatsPerBar == 0)
        {
            OnBar?.Invoke();
        }
        OnBeat?.Invoke();
    }
}
