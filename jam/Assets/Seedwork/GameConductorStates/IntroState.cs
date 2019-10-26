using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroState : IState
{
    public IState NextState { get; private set; }

    public void OnEnter()
    {
        GameConductor.CameraStateTrigger("Initialize");
        MusicBox.ChangeMusic(Song.Game.ToInt());
    }

    public IEnumerator OnUpdate()
    {
        yield return new WaitForSeconds(1);
        bool isFinished = false;
        ScreenFader.FadeInThen(() =>
        {
            Juicer.ShakeCamera(0.5f);
            MessageController.AddMessage("yo.");
            MessageController.AddMessage("this is game peanutbutters.");
            MessageController.AddMessage("it goes great with game jams.");
            MessageController.AddMessage("you may have noticed the camera and music change. very easy to do with cinemachine and the juicer.", postAction: () =>
            {
                GameConductor.CameraStateTrigger("NextState");
            });
            MessageController.AddMessage("these bois are moving with the simplemover script. it's an easy way to get some nice motion with little effort. useful for pickups, simple enemies, etc.", postAction: () =>
            {
                GameConductor.CameraStateTrigger("NextState");
            });
            MessageController.AddMessage("this is a fun script called textjacker. it jacks up text.", postAction: () =>
            {
                GameConductor.CameraStateTrigger("NextState");
            });
            MessageController.AddMessage("after this message finishes, the game will transition to the next game state. a basic player object will be created.", postAction: () => isFinished = true);
        });
        do
        {
            yield return new WaitForSeconds(1);
        } while (!isFinished);
    }

    public void OnExit()
    {
        // MainGameState is implemented as a scriptableobject, so get it from GameConductor
        NextState = GameConductor.GetScriptableGameStateOfType<MainGameState>();
    }
}
