using Unity.VisualScripting;
using UnityEngine;

public class Player_JumpState : Player_GroundedState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player_JumpState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }

    public override void Update()
    {
       
    }
    public override void FixedUpdate()
    {
      
        
    }
}
