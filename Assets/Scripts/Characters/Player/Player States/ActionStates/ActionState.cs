using UnityEngine;


public abstract class ActionState : CharacterBaseState
{
    protected readonly Player player;

    protected ActionState(StateMachine stateMachine, string animBoolName, Player player) : base(stateMachine, animBoolName){
        this.player = player;
    }
}
