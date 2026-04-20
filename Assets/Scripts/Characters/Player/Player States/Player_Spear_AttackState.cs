using UnityEngine;

public class Player_Spear_AttackState : PlayerBaseState
{
    public Player_Spear_AttackState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    { 


    }
    private RaycastHit2D[] hits;
    private float combotime;

    public override void Enter(){
        base.Enter();
        hits = Physics2D.CircleCastAll(player.rb.position, 25f,player.transform.right,0f,LayerMask.NameToLayer("Enemy"));
        combotime = 0.5f;
      
    }
     public override void Update(){
        combotime-= Time.deltaTime;
        if(player.GetAttackPressedInput() && combotime >=0 ){
            //stateMachine.ChangeState();

        }
        if(combotime <=0 ){
            stateMachine.ChangeState(stateMachine.previousState);
        }
     }



}