using UnityEngine;

public class Player_AttackState : PlayerBaseState
{
    public Player_AttackState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    { }

   
    public override void Enter(){
        base.Enter();
 
    }
    public override void FixedUpdate()
    {
        base.Update();
   
        
    }
    
    
}
