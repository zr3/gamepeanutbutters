using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class MusicBoxPlayable : PlayableBehaviour
{
    private Playable mixer;
    private double currentTime, channelCutOverTime, channelPlayTime;
    private LinkedList<MusicChannel> musicChannels = new LinkedList<MusicChannel>();

    public void LoadAndPlay(MusicClip musicClip, Playable owner, PlayableGraph graph)
    {
        owner.SetInputCount(1);
        mixer = AudioMixerPlayable.Create(graph, musicClip.ClipLayers.Length);
        mixer.SetInputCount(musicChannels.Count + 1);
        var nextMusicChannelPlayable = ScriptPlayable<MusicChannel>.Create(graph);
        var nextMusicChannel = nextMusicChannelPlayable.GetBehaviour();
        currentTime = 0;
        var leadTime = musicClip.Beginning.InSeconds(musicClip.BeatsPerBar, musicClip.BPM);
        channelCutOverTime = musicChannels.Count == 0 ? double.MinValue : musicChannels.First.Value.NextEnding(leadTime);
        channelPlayTime = musicChannels.Count == 0 ? double.MinValue : channelCutOverTime - leadTime;
        musicChannels.AddLast(nextMusicChannel);
        for (int i = 0; i < musicChannels.Count + 1; i++)
        {
            graph.Connect(mixer, 0, owner, 0);
            graph.Connect(nextMusicChannelPlayable, 0, mixer, i);
            mixer.SetInputWeight(i, 1);
        }
    }

    public override void PrepareFrame(Playable owner, FrameData info)
    {
        // early exit
        if (mixer.GetInputCount() == 0 || musicChannels.Count == 0) return;

        // cut channels
        currentTime += info.deltaTime;
        if (currentTime >= channelPlayTime)
        {
            if (musicChannels.Count > 1)
            {
                musicChannels.First.Next.Value.Play();
            } else
            {
                musicChannels.First.Value.Play();
            }
            channelPlayTime = double.MaxValue;
        }
        if (musicChannels.Count > 1 && currentTime >= channelCutOverTime)
        {
            musicChannels.First.Value.Stop();
            musicChannels.RemoveFirst();
            channelCutOverTime = double.MaxValue;
        }

        base.PrepareFrame(owner, info);
    }
}
