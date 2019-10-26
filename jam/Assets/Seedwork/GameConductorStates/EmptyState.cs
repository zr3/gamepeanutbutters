using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyState : IState
{
    public IState NextState { get; private set; }

    public void OnEnter() { }

    public IEnumerator OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit() { }
}
