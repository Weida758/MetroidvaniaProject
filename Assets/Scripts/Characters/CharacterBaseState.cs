using UnityEngine;

public class CharacterBaseState
{

    protected readonly StateMachine stateMachine;
    protected readonly string animBoolName;
    public CharacterBaseState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }


    public virtual void Enter()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void Exit()
    {
        
    }
    
}
