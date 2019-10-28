using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicClip.asset", menuName = "peanutbutters/MusicClip", order = 0)]
public class MusicClip : ScriptableObject
{
    public string Name;
    public Layer[] ClipLayers;
    public int BPM = 120;
    public int BeatsPerBar = 4;
    public MusicLocation Beginning;
    public MusicLocation[] Endings;
    public MusicLocation IntroEnd;
    public MusicLocation VampEnd;

    [Serializable]
    public class MusicLocation
    {
        public int Bar = 1;
        public int Beat = 1;

        public int InBeats(int BeatsPerBar) => ((Bar - 1) * BeatsPerBar) + Beat;
        public double InSeconds(int BeatsPerBar, int BPM)
        {
            int beatOffset = InBeats(BeatsPerBar) - 1;
            double secondsPerBeat = 60.0 / BPM;
            return beatOffset * secondsPerBeat;
        }
    }

    [Serializable]
    public class Layer
    {
        public AudioClip Clip;
    }

    public bool NeedsSpecialStart =>
        Beginning.Bar != 1
        || Beginning.Beat != 1;

    public bool NeedsSpecialLooping =>
        IntroEnd.Bar != 1
        || IntroEnd.Beat != 1
        || VampEnd.Bar != Endings[Endings.Length - 1].Bar
        || VampEnd.Beat != Endings[Endings.Length - 1].Beat;

    public double LoopLength => VampEnd.InSeconds(BeatsPerBar, BPM) - IntroEnd.InSeconds(BeatsPerBar, BPM);
}
