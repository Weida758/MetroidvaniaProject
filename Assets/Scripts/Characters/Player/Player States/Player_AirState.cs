using UnityEngine;

public class Player_AirState : PlayerBaseState
{
    public Player_AirState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    { }

    public bool WallCheck(){
        //Debug.DrawRay(player.rb.transform.position, player.transform.TransformDirection(Vector3.right) *0.75f,Color.red);
        return Physics2D.Raycast(player.rb.transform.position, new Vector2(player.getFacingDirection(),0), 0.75f ,1 << LayerMask.NameToLayer("Wall"));

    }
    
    
    
}
