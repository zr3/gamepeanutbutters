using System.Collections;

public interface IState
{
    void OnEnter();
    void OnExit();
    IEnumerator OnUpdate();
    IState NextState { get; }
}
