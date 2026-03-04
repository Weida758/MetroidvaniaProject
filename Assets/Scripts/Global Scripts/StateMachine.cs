using UnityEngine;

public class StateMachine
{
    public CharacterBaseState currentState { get; private set; }
    

    public void Initialize(CharacterBaseState state)
    {
        currentState = state;
    }

    public void ChangeState(CharacterBaseState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }

    public void FixedUpdateActiveState()
    {
        currentState.FixedUpdate();
    }
}
