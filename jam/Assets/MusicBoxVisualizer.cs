using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MusicBoxVisualizer : MonoBehaviour
{
    public Text TextToUpdate;

    private MusicClip lastClip;

    public void OnBeatUpdate()
    {
        MusicClip clip = MusicBox.CurrentClip;
        if (clip != lastClip)
        {
            lastClip = clip;
        }
        int numBeats = clip.Endings.Last().InBeats(clip.BeatsPerBar) - clip.Beginning.InBeats(clip.BeatsPerBar);
        var timeView = new StringBuilder();
        var beatView = new StringBuilder();
        var alignView = new StringBuilder();
        var loopView = new StringBuilder();
        int beginningBeat = clip.Beginning.InBeats(clip.BeatsPerBar) - 1;
        int[] endingBeats = clip.Endings.Select(e => e.InBeats(clip.BeatsPerBar) - 1).ToArray();
        int introBeat = clip.IntroEnd.InBeats(clip.BeatsPerBar) - 1;
        int vampBeat = clip.VampEnd.InBeats(clip.BeatsPerBar) - 1;

        for (int i = 0; i <= numBeats; ++i)
        {
            if (i < numBeats)
            {
                timeView.Append(i == MusicBox.CurrentBeat ? '*' : '.');
                beatView.Append(i % clip.BeatsPerBar == 0 ? '|' : '-');
            }
            alignView.Append(beginningBeat == i ? 'B' : endingBeats.Contains(i) ? 'E' : ' ');
            loopView.Append(introBeat == i ? 'I' : vampBeat == i ? 'V' : ' ');
        }
        char channel = MusicBox.UsingChannelA ? 'A' : 'B';
        var result = new StringBuilder()
            .Append("   Time: ").AppendLine(timeView.ToString())
            .Append("   Clip: ").AppendLine(string.IsNullOrWhiteSpace(clip.Name) ? "(unnamed)" : clip.Name)
            .Append($" Chan {channel}: ").AppendLine(beatView.ToString())
            .Append("  align: ").AppendLine(alignView.ToString())
            .Append("   loop: ").AppendLine(loopView.ToString());

        TextToUpdate.text = result.ToString();
    }
}
