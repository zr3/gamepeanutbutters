using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Playables;

public class SimpleMoverPlayable : PlayableBehaviour
{
    /// <summary>
    ///     This method is invoked when one of the following situations occurs: <br><br>
    ///     The effective play state during traversal is changed to Playables.PlayState.Paused.
    ///     This state is indicated by FrameData.effectivePlayState.<br><br> The PlayableGraph
    ///     is stopped while the playable play state is Playing. This state is indicated
    ///     by PlayableGraph.IsPlaying returning true.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    // public override void OnBehaviourPause(Playable playable, FrameData info) { }

    /// <summary>
    ///     This function is called when the Playable play state is changed to Playables.PlayState.Playing.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    // public override void OnBehaviourPlay(Playable playable, FrameData info) { }

    /// <summary>
    ///     This function is called when the PlayableGraph that owns this PlayableBehaviour
    ///     starts.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    // public override void OnGraphStart(Playable playable) { }

    /// <summary>
    ///     This function is called when the PlayableGraph that owns this PlayableBehaviour
    ///     stops.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    // public override void OnGraphStop(Playable playable) { }

    /// <summary>
    ///     This function is called when the Playable that owns the PlayableBehaviour is
    ///     created.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    // public override void OnPlayableCreate(Playable playable) { }

    /// <summary>
    ///     This function is called when the Playable that owns the PlayableBehaviour is
    ///     destroyed.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    // public override void OnPlayableDestroy(Playable playable) { }

    /// <summary>
    ///     This function is called during the PrepareData phase of the PlayableGraph.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    // public override void PrepareData(Playable playable, FrameData info) { }

    /// <summary>
    ///     This function is called during the PrepareFrame phase of the PlayableGraph.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    // public override void PrepareFrame(Playable playable, FrameData info) { }

    /// <summary>
    ///     This function is called during the ProcessFrame phase of the PlayableGraph.
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    /// <param name="playerData">The user data of the ScriptPlayableOutput that initiated the process pass.</param>
    // public override void ProcessFrame(Playable playable, FrameData info, object playerData) { }

}
