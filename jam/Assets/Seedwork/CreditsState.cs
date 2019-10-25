using System.Collections;
using UnityEngine;

public class CreditsState : IState
{
    public IState NextState { get; private set; }

    public void OnEnter() {
        MusicBox.ChangeMusic((int)Song.Final);
        CreditRoller.RollCredits();
    }

    public IEnumerator OnUpdate()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
    }

    public void OnExit()
    {
        // restart game
        CreditRoller.ResetCredits();
        NextState = new IntroState();
    }
}
