using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class MusicChannel : PlayableBehaviour
{
    public MusicClip Clip { get; private set; }
    public double NextEnding(double leadTime) => Clip
            .Endings
            .First(e => e.InSeconds(Clip.BeatsPerBar, Clip.BPM) > currentTimeInClipSpace + leadTime)
            .InSeconds(Clip.BeatsPerBar, Clip.BPM);
    public Action OnFinished;

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

    public void Play(double delay = 0)
    {
        for (int i = 0; i < inputPlayables.Length; i++)
        {
            inputPlayables[i].Seek(0, delay);
        }
        clipLoopbackToTime = Clip.IntroEnd.InSeconds(Clip.BeatsPerBar, Clip.BPM);
        clipLoopbackFromTime = Clip.VampEnd.InSeconds(Clip.BeatsPerBar, Clip.BPM);
        currentTimeInClipSpace = 0;
        calculatedLoopTime = Clip.LoopLength;
        endTime = double.MaxValue;
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

    public override void PrepareFrame(Playable owner, FrameData info)
    {
        // early exit
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

        base.PrepareFrame(owner, info);
    }
}
